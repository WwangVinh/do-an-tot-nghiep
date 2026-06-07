using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Customer;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Interfaces.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicBusiness.Services.Customer
{
    public class ConsultRequestService : IConsultRequestService
    {
        private readonly IConsultRequestRepository _repo;
        private readonly ICarRepository _carRepo;
        private readonly INotificationService _notiService;
        private readonly ICarPricingVersionRepository _versionRepo;
        private readonly ICarColorRepository _colorRepo;

        public ConsultRequestService(
            IConsultRequestRepository repo,
            ICarRepository carRepo,
            INotificationService notiService,
            ICarPricingVersionRepository versionRepo,
            ICarColorRepository colorRepo)
        {
            _repo = repo;
            _carRepo = carRepo;
            _notiService = notiService;
            _versionRepo = versionRepo;
            _colorRepo = colorRepo;
        }

        public async Task<(bool Success, string Message)> CreateAsync(ConsultRequestCreateDto dto)
        {
            if (dto.RequestType != ConsultRequestType.Quotation &&
                dto.RequestType != ConsultRequestType.Installment)
                return (false, "Loại yêu cầu không hợp lệ.");

            var car = await _carRepo.GetByIdAsync(dto.CarId);
            if (car == null || car.IsDeleted)
                return (false, "Xe không tồn tại hoặc đã ngừng kinh doanh!");

            // ===== Validate field tài chính cho trả góp =====
            if (dto.RequestType == ConsultRequestType.Installment)
            {
                if (dto.MonthlyIncome is null or <= 0)
                    return (false, "Vui lòng nhập thu nhập hàng tháng để được tư vấn gói vay phù hợp.");
                if (dto.DownPayment is null or < 0)
                    return (false, "Vui lòng nhập số tiền trả trước dự kiến.");
                if (dto.LoanTermMonths is null or <= 0)
                    return (false, "Vui lòng chọn kỳ hạn vay mong muốn.");
            }

            // ===== ✨ MỚI: Xử lý phiên bản và màu xe qua Repository =====
            int? finalPricingVersionId = dto.CarPricingVersionId;
            int? finalCarColorId = dto.CarColorId;

            var carVersions = await _versionRepo.GetVersionsByCarIdAsync(dto.CarId);
            var carColors = await _colorRepo.GetColorsByCarIdAsync(dto.CarId);

            // Validate phiên bản
            if (finalPricingVersionId.HasValue)
            {
                var validVersion = carVersions.Any(v => v.PricingVersionId == finalPricingVersionId.Value);
                if (!validVersion) return (false, "Phiên bản xe không hợp lệ. Vui lòng chọn lại.");
            }
            else
            {
                // Auto-default: lấy bản giá thấp nhất
                finalPricingVersionId = carVersions.OrderBy(v => v.PriceVnd)
                                                   .Select(v => (int?)v.PricingVersionId)
                                                   .FirstOrDefault();
            }

            // Validate màu xe
            if (finalCarColorId.HasValue)
            {
                var validColor = carColors.Any(c => c.CarColorId == finalCarColorId.Value);
                if (!validColor) return (false, "Màu xe không hợp lệ. Vui lòng chọn lại.");
            }
            else
            {
                // Auto-default: lấy màu đầu tiên
                finalCarColorId = carColors.OrderBy(c => c.CarColorId)
                                           .Select(c => (int?)c.CarColorId)
                                           .FirstOrDefault();
            }

            var entity = new ConsultRequest
            {
                CarId = dto.CarId,
                ShowroomId = dto.ShowroomId,
                CustomerName = dto.CustomerName.Trim(),
                Phone = dto.Phone.Trim(),
                RequestType = dto.RequestType,
                CustomerNote = dto.CustomerNote?.Trim(),
                MonthlyIncome = dto.RequestType == ConsultRequestType.Installment ? dto.MonthlyIncome : null,
                DownPayment = dto.RequestType == ConsultRequestType.Installment ? dto.DownPayment : null,
                LoanTermMonths = dto.RequestType == ConsultRequestType.Installment ? dto.LoanTermMonths : null,
                CarPricingVersionId = finalPricingVersionId,
                CarColorId = finalCarColorId,
                Status = ConsultStatus.Pending,
                UserId = null,
                CreatedAt = DateTime.Now
            };

            await _repo.AddAsync(entity);

            // Lấy tên phiên bản & màu để hiển thị trong notification
            var versionName = finalPricingVersionId.HasValue
                ? carVersions.FirstOrDefault(v => v.PricingVersionId == finalPricingVersionId.Value)?.VersionName
                : null;

            var colorName = finalCarColorId.HasValue
                ? carColors.FirstOrDefault(c => c.CarColorId == finalCarColorId.Value)?.ColorName
                : null;

            var typeLabel = dto.RequestType == ConsultRequestType.Quotation ? "báo giá" : "mua trả góp";
            var emoji = dto.RequestType == ConsultRequestType.Quotation ? "💰" : "🏦";

            // Build content notification có cả phiên bản + màu nếu có
            var carDesc = $"{car.Brand} {car.Name}";
            if (!string.IsNullOrWhiteSpace(versionName)) carDesc += $" - bản {versionName}";
            if (!string.IsNullOrWhiteSpace(colorName)) carDesc += $" - màu {colorName}";

            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: dto.ShowroomId,
                roleTarget: $"{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales}",
                title: $"Yêu cầu {typeLabel} mới! {emoji}",
                content: $"Khách {dto.CustomerName} ({dto.Phone}) vừa gửi yêu cầu {typeLabel} cho xe {carDesc}.",
                actionUrl: "/consult-requests",
                type: "ConsultRequest"
            );

            var successMsg = dto.RequestType == ConsultRequestType.Quotation
                ? "Đã gửi yêu cầu báo giá thành công! Nhân viên sẽ liên hệ tư vấn cho bạn sớm nhé."
                : "Đã gửi yêu cầu mua trả góp thành công! Nhân viên sẽ liên hệ tư vấn gói vay cho bạn sớm nhé.";

            return (true, successMsg);
        }

        public async Task<IEnumerable<object>> GetByPhoneAsync(string phone)
        {
            var items = await _repo.GetByPhoneAsync(phone);

            return items.Select(x => new
            {
                x.ConsultRequestId,
                x.RequestType,
                RequestTypeLabel = ToRequestTypeLabel(x.RequestType),
                x.Status,
                StatusLabel = ToStatusLabel(x.Status),
                CarName = x.Car?.Name,
                CarImage = x.Car?.ImageUrl,
                ShowroomName = x.Showroom?.Name,
                VersionName = x.CarPricingVersion?.VersionName,
                ColorName = x.CarColor?.ColorName,
                CreatedAt = x.CreatedAt?.ToString("dd/MM/yyyy HH:mm")
            });
        }

        public async Task<object?> GetDetailByPhoneAsync(int id, string phone)
        {
            var x = await _repo.GetByIdAsync(id);
            if (x == null || x.Phone != phone)
                return null;

            return new
            {
                x.ConsultRequestId,
                x.CustomerName,
                x.Phone,
                x.RequestType,
                RequestTypeLabel = ToRequestTypeLabel(x.RequestType),
                x.Status,
                StatusLabel = ToStatusLabel(x.Status),
                x.CustomerNote,
                HasInstallmentInfo = x.RequestType == ConsultRequestType.Installment,
                InstallmentInfo = x.RequestType == ConsultRequestType.Installment
                    ? new
                    {
                        x.MonthlyIncome,
                        x.DownPayment,
                        x.LoanTermMonths
                    }
                    : null,
                CarDetails = new
                {
                    x.Car?.CarId,
                    x.Car?.Name,
                    x.Car?.Brand,
                    ImageUrl = x.Car?.ImageUrl,
                    VersionId = x.CarPricingVersionId,
                    VersionName = x.CarPricingVersion?.VersionName,
                    VersionPrice = x.CarPricingVersion?.PriceVnd,
                    ColorId = x.CarColorId,
                    ColorName = x.CarColor?.ColorName,
                    ColorHex = x.CarColor?.HexCode
                },
                ShowroomDetails = new
                {
                    x.Showroom?.ShowroomId,
                    x.Showroom?.Name,
                    x.Showroom?.District,
                    x.Showroom?.StreetAddress
                },
                CreatedAt = x.CreatedAt?.ToString("dd/MM/yyyy HH:mm"),
                UpdatedAt = x.UpdatedAt?.ToString("dd/MM/yyyy HH:mm"),
                Timeline = BuildTimeline(x.Status)
            };
        }

        public async Task<(bool Success, string Message)> CancelByPhoneAsync(int id, string phone, string? reason)
        {
            var x = await _repo.GetByIdAsync(id);

            if (x == null || x.Phone != phone)
                return (false, "Không tìm thấy yêu cầu hoặc số điện thoại không khớp.");

            if (x.Status == ConsultStatus.Consulting)
                return (false, "Nhân viên đang tư vấn cho bạn, vui lòng liên hệ trực tiếp showroom để hủy nhé!");

            if (x.Status == ConsultStatus.Success)
                return (false, "Yêu cầu này đã được xử lý xong, không thể hủy.");

            if (x.Status == ConsultStatus.Failed)
                return (false, "Yêu cầu này đã đóng, không thể hủy.");

            if (x.Status == ConsultStatus.Cancelled)
                return (false, "Yêu cầu này đã bị hủy trước đó rồi.");

            x.Status = ConsultStatus.Cancelled;
            x.UpdatedAt = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(reason))
            {
                var line = $"[{DateTime.Now:dd/MM/yyyy HH:mm} - Khách hủy]: {reason.Trim()}";
                x.Note = string.IsNullOrWhiteSpace(x.Note) ? line : $"{x.Note}\n{line}";
            }

            await _repo.UpdateAsync(x);

            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: x.ShowroomId,
                roleTarget: $"{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales}",
                title: "Khách tự hủy yêu cầu ❌",
                content: $"Khách {x.CustomerName} ({x.Phone}) vừa tự hủy yêu cầu {ToRequestTypeLabel(x.RequestType)} xe {x.Car?.Name}."
                    + (string.IsNullOrWhiteSpace(reason) ? "" : $" Lý do: {reason}"),
                actionUrl: "/consult-requests",
                type: "ConsultRequest"
            );

            return (true, "Đã hủy yêu cầu thành công!");
        }

        private static string ToRequestTypeLabel(string? type) => type switch
        {
            ConsultRequestType.Quotation => "Báo giá",
            ConsultRequestType.Installment => "Mua trả góp",
            _ => type ?? "Không xác định"
        };

        private static string ToStatusLabel(string? status) => status switch
        {
            ConsultStatus.Pending => "Chờ nhân viên tiếp nhận",
            ConsultStatus.Consulting => "Đang được tư vấn",
            ConsultStatus.Success => "Tư vấn thành công",
            ConsultStatus.Failed => "Tư vấn không thành công",
            ConsultStatus.Cancelled => "Đã hủy",
            _ => status ?? "Không xác định"
        };

        private static object BuildTimeline(string? currentStatus)
        {
            if (currentStatus == ConsultStatus.Cancelled ||
                currentStatus == ConsultStatus.Failed ||
                currentStatus == ConsultStatus.Success)
            {
                return new
                {
                    IsTerminated = true,
                    Status = currentStatus,
                    Label = ToStatusLabel(currentStatus)
                };
            }

            var steps = new[]
            {
                ConsultStatus.Pending,
                ConsultStatus.Consulting
            };

            int currentIndex = Array.IndexOf(steps, currentStatus);

            return new
            {
                IsTerminated = false,
                Steps = steps.Select((s, i) => new
                {
                    Status = s,
                    Label = ToStatusLabel(s),
                    IsDone = i < currentIndex,
                    IsCurrent = i == currentIndex,
                    IsPending = i > currentIndex
                })
            };
        }
    }
}