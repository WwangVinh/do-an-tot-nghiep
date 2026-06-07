using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Interfaces.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicBusiness.Services.Admin
{
    public class ConsultRequestAdminService : IConsultRequestAdminService
    {
        private readonly IConsultRequestRepository _repo;
        private readonly INotificationService _notiService;

        public ConsultRequestAdminService(
            IConsultRequestRepository repo,
            INotificationService notiService)
        {
            _repo = repo;
            _notiService = notiService;
        }

        private bool HasShowroomAccess(string userRole, int? userShowroomId, int? requestShowroomId)
        {
            if (userRole == AppRoles.Admin) return true;
            return userShowroomId.HasValue && requestShowroomId == userShowroomId.Value;
        }

        private bool IsSalesSide(string userRole) =>
            userRole == AppRoles.Admin ||
            userRole == AppRoles.Manager ||
            userRole == AppRoles.Sales ||
            userRole == AppRoles.ShowroomSales;

        // Chỉ Sales đã pickup, hoặc Manager/Admin được can thiệp request đã có người nhận
        private bool CanModifyAssigned(string userRole, int? assignedUserId, int currentUserId)
        {
            if (userRole == AppRoles.Admin || userRole == AppRoles.Manager) return true;
            return assignedUserId.HasValue && assignedUserId.Value == currentUserId;
        }

        private string AppendNote(string? existingNote, string userRole, string content)
        {
            var line = $"[{DateTime.Now:dd/MM/yyyy HH:mm} - {userRole}]: {content.Trim()}";
            return string.IsNullOrWhiteSpace(existingNote) ? line : $"{existingNote}\n{line}";
        }

        public async Task<object> GetListAsync(
            int page, int pageSize, string? search, string? status, string? requestType,
            string userRole, int? userShowroomId, int currentUserId)
        {
            if (userRole != AppRoles.Admin && !userShowroomId.HasValue)
                return new { TotalCount = 0, Data = new List<object>() };

            int? filterShowroom = (userRole == AppRoles.Admin) ? null : userShowroomId;
            var (items, total) = await _repo.GetAdminListAsync(
                page, pageSize, search, status, requestType, filterShowroom, null);

            return new
            {
                TotalCount = total,
                Data = items.Select(x => new
                {
                    x.ConsultRequestId,
                    x.CustomerName,
                    x.Phone,
                    x.RequestType,
                    x.Status,
                    CarName = x.Car?.Name,
                    ShowroomName = x.Showroom?.Name,
                    AssignedTo = x.User?.FullName,
                    AssignedUserId = x.UserId,
                    IsMine = x.UserId.HasValue && x.UserId.Value == currentUserId,
                    CreatedAt = x.CreatedAt?.ToString("dd/MM/yyyy HH:mm")
                })
            };
        }

        public async Task<object?> GetDetailAsync(int id, string userRole, int? userShowroomId)
        {
            var x = await _repo.GetByIdAsync(id);
            if (x == null) return null;

            if (!HasShowroomAccess(userRole, userShowroomId, x.ShowroomId))
                return null;

            return new
            {
                x.ConsultRequestId,
                x.CustomerName,
                x.Phone,
                x.RequestType,
                x.Status,
                x.CustomerNote,
                x.Note,
                InstallmentInfo = x.RequestType == ConsultRequestType.Installment
                    ? new
                    {
                        x.MonthlyIncome,
                        x.DownPayment,
                        x.LoanTermMonths
                    }
                    : null,
                CarDetails = new { x.Car?.CarId, x.Car?.Name, ImageUrl = x.Car?.ImageUrl },
                ShowroomDetails = new { x.Showroom?.ShowroomId, x.Showroom?.Name, x.Showroom?.Province },
                AssignedTo = x.User?.FullName,
                AssignedUserId = x.UserId,
                x.CreatedAt,
                x.UpdatedAt
            };
        }

        public async Task<(bool Success, string Message)> PickupAsync(
            int id, ConsultPickupDto dto, int currentUserId, string userRole, int? userShowroomId)
        {
            if (!IsSalesSide(userRole))
                return (false, "Chỉ Sales mới được tiếp nhận yêu cầu tư vấn.");

            var x = await _repo.GetByIdAsync(id);
            if (x == null) return (false, "Không tìm thấy yêu cầu.");

            if (!HasShowroomAccess(userRole, userShowroomId, x.ShowroomId))
                return (false, "Bạn không có quyền tiếp nhận yêu cầu của chi nhánh khác!");

            if (x.Status != ConsultStatus.Pending)
                return (false, $"Yêu cầu đang ở trạng thái '{x.Status}', không thể tiếp nhận.");

            x.Status = ConsultStatus.Consulting;
            x.UserId = currentUserId;
            x.Note = AppendNote(x.Note, userRole,
                $"Tiếp nhận tư vấn{(string.IsNullOrWhiteSpace(dto.Note) ? "" : $": {dto.Note}")}");
            x.UpdatedAt = DateTime.Now;

            await _repo.UpdateAsync(x);

            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: x.ShowroomId,
                roleTarget: AppRoles.Manager,
                title: "Sales đã nhận yêu cầu tư vấn ✅",
                content: $"Yêu cầu của khách {x.CustomerName} ({x.Phone}) - xe {x.Car?.Name} đã được tiếp nhận.",
                actionUrl: "/consult-requests",
                type: "ConsultRequest"
            );

            return (true, "Đã tiếp nhận yêu cầu, bắt đầu tư vấn cho khách nhé!");
        }

        public async Task<(bool Success, string Message)> SubmitResultAsync(
            int id, ConsultResultDto dto, int currentUserId, string userRole, int? userShowroomId)
        {
            if (!IsSalesSide(userRole))
                return (false, "Bạn không có quyền cập nhật kết quả tư vấn.");

            if (string.IsNullOrWhiteSpace(dto.ResultNote))
                return (false, "Vui lòng nhập nội dung/lý do kết quả tư vấn.");

            var x = await _repo.GetByIdAsync(id);
            if (x == null) return (false, "Không tìm thấy yêu cầu.");

            if (!HasShowroomAccess(userRole, userShowroomId, x.ShowroomId))
                return (false, "Bạn không có quyền can thiệp yêu cầu của chi nhánh khác!");

            if (x.Status != ConsultStatus.Consulting)
                return (false, $"Yêu cầu đang ở trạng thái '{x.Status}', cần tiếp nhận trước khi chốt kết quả.");

            // Sales chỉ chốt kết quả request mà chính mình pickup
            if (!CanModifyAssigned(userRole, x.UserId, currentUserId))
                return (false, "Yêu cầu này đang do Sales khác phụ trách, bạn không can thiệp được.");

            if (dto.IsSuccess)
            {
                x.Status = ConsultStatus.Success;
                x.Note = AppendNote(x.Note, userRole, $"Tư vấn thành công: {dto.ResultNote}");
            }
            else
            {
                x.Status = ConsultStatus.Failed;
                x.Note = AppendNote(x.Note, userRole, $"Tư vấn không thành công: {dto.ResultNote}");
            }

            x.UpdatedAt = DateTime.Now;
            await _repo.UpdateAsync(x);

            var typeLabel = x.RequestType == ConsultRequestType.Quotation ? "báo giá" : "trả góp";

            if (dto.IsSuccess)
            {
                await _notiService.CreateNotificationAsync(
                    userId: null,
                    showroomId: x.ShowroomId,
                    roleTarget: AppRoles.Manager,
                    title: $"Tư vấn {typeLabel} thành công 🎉",
                    content: $"Khách {x.CustomerName} ({x.Phone}) - xe {x.Car?.Name} đã chốt sau khi tư vấn {typeLabel}.",
                    actionUrl: "/consult-requests",
                    type: "ConsultRequest"
                );
            }
            else
            {
                await _notiService.CreateNotificationAsync(
                    userId: null,
                    showroomId: x.ShowroomId,
                    roleTarget: AppRoles.Manager,
                    title: $"Tư vấn {typeLabel} không thành công",
                    content: $"Yêu cầu của khách {x.CustomerName} ({x.Phone}) - xe {x.Car?.Name} chưa chốt được. Lý do: {dto.ResultNote}",
                    actionUrl: "/consult-requests",
                    type: "ConsultRequest"
                );
            }

            return (true, dto.IsSuccess
                ? "Đã ghi nhận tư vấn thành công."
                : "Đã ghi nhận tư vấn không thành công.");
        }

        public async Task<(bool Success, string Message)> CancelByAdminAsync(
            int id, string cancelReason, int currentUserId, string userRole, int? userShowroomId)
        {
            if (string.IsNullOrWhiteSpace(cancelReason))
                return (false, "Vui lòng nhập lý do hủy.");

            var x = await _repo.GetByIdAsync(id);
            if (x == null) return (false, "Không tìm thấy yêu cầu.");

            if (!HasShowroomAccess(userRole, userShowroomId, x.ShowroomId))
                return (false, "Bạn không có quyền hủy yêu cầu của chi nhánh khác!");

            if (x.Status == ConsultStatus.Success ||
                x.Status == ConsultStatus.Failed ||
                x.Status == ConsultStatus.Cancelled)
                return (false, $"Yêu cầu đang ở trạng thái '{x.Status}', không thể hủy.");

            // Nếu đã có người pickup mà Sales khác đòi hủy → chặn
            if (x.Status == ConsultStatus.Consulting &&
                !CanModifyAssigned(userRole, x.UserId, currentUserId))
                return (false, "Yêu cầu này đang do Sales khác phụ trách, bạn không hủy được.");

            x.Status = ConsultStatus.Cancelled;
            x.Note = AppendNote(x.Note, userRole, $"Hủy yêu cầu: {cancelReason}");
            x.UpdatedAt = DateTime.Now;

            await _repo.UpdateAsync(x);

            if (userRole != AppRoles.Manager && userRole != AppRoles.Admin)
            {
                await _notiService.CreateNotificationAsync(
                    userId: null,
                    showroomId: x.ShowroomId,
                    roleTarget: AppRoles.Manager,
                    title: "Cảnh báo: Yêu cầu tư vấn bị hủy",
                    content: $"Nhân viên [{userRole}] vừa hủy yêu cầu của khách {x.CustomerName}. Lý do: {cancelReason}",
                    actionUrl: "/consult-requests",
                    type: "SystemAlert"
                );
            }

            return (true, "Đã hủy yêu cầu thành công.");
        }

        public async Task<Dictionary<string, int>> GetStatsAsync(string userRole, int? userShowroomId)
        {
            int? filterShowroom = (userRole == AppRoles.Admin) ? null : userShowroomId;
            return await _repo.CountByStatusAsync(filterShowroom);
        }
    }
}