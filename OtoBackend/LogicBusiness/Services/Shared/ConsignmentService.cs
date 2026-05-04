using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Interfaces.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicBusiness.Services.Shared
{
    public class ConsignmentService : IConsignmentService
    {
        private readonly IConsignmentRepository _consignRepo;
        private readonly INotificationService _notiService;

        private static readonly string[] AllowedRoles =
        {
            AppRoles.Admin, AppRoles.Manager, AppRoles.Sales, AppRoles.ShowroomSales
        };

        public ConsignmentService(IConsignmentRepository consignRepo, INotificationService notiService)
        {
            _consignRepo = consignRepo;
            _notiService = notiService;
        }

        public async Task<(bool Success, string Message)> CreateConsignmentAsync(ConsignmentCreateDto dto)
        {
            var duplicate = await _consignRepo.GetPendingByPhoneAndCarAsync(
                dto.GuestPhone.Trim(),
                dto.Brand.Trim(),
                dto.Model.Trim(),
                dto.Year
            );

            if (duplicate != null)
                return (false, "Xe này đã có yêu cầu ký gửi đang chờ xử lý. Vui lòng chờ nhân viên liên hệ.");

            var consignment = new Consignment
            {
                GuestName = dto.GuestName.Trim(),
                GuestPhone = dto.GuestPhone.Trim(),
                GuestEmail = dto.GuestEmail?.Trim(),
                Brand = dto.Brand.Trim(),
                Model = dto.Model.Trim(),
                Year = dto.Year,
                Mileage = dto.Mileage,
                ConditionDescription = dto.ConditionDescription?.Trim(),
                ExpectedPrice = dto.ExpectedPrice,
                Status = ConsignmentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _consignRepo.AddAsync(consignment);

            await _notiService.CreateNotificationAsync(
                userId: null,
                roleTarget: AppRoles.Manager,
                showroomId: null,
                title: "Yêu cầu ký gửi xe mới 🚗",
                content: $"Khách {dto.GuestName} ({dto.GuestPhone}) muốn ký gửi {dto.Brand} {dto.Model} ({dto.Year}). Vào kiểm tra ngay!",
                actionUrl: "/consignments",
                type: "System"
            );

            return (true, "Gửi yêu cầu ký gửi thành công! Showroom sẽ liên hệ bạn sớm nhất để thẩm định xe.");
        }

        public async Task<IEnumerable<ConsignmentListItemDto>> GetAllAdminAsync()
        {
            var list = await _consignRepo.GetAllAdminAsync();
            return list.Select(MapToListItem);
        }

        public async Task<ConsignmentResponseDto?> GetByIdAsync(int consignmentId)
        {
            var c = await _consignRepo.GetByIdAsync(consignmentId);
            return c == null ? null : MapToResponse(c);
        }

        public async Task<(bool Success, string Message)> UpdateConsignmentStatusAsync(int consignmentId, ConsignmentUpdateDto dto, string adminRole)
        {
            var consignment = await _consignRepo.GetByIdAsync(consignmentId);
            if (consignment == null)
                return (false, "Không tìm thấy hồ sơ ký gửi này.");

            if (!AllowedRoles.Contains(adminRole))
                return (false, "Bạn không có quyền cập nhật hồ sơ ký gửi.");

            if (!ConsignmentStatus.IsValid(dto.Status))
                return (false, $"Trạng thái '{dto.Status}' không hợp lệ. Các trạng thái được phép: {string.Join(", ", ConsignmentStatus.All)}.");

            consignment.Status = dto.Status;
            if (dto.AgreedPrice.HasValue) consignment.AgreedPrice = dto.AgreedPrice.Value;
            if (dto.CommissionRate.HasValue) consignment.CommissionRate = dto.CommissionRate.Value;
            if (dto.LinkedCarId.HasValue) consignment.LinkedCarId = dto.LinkedCarId.Value;
            consignment.UpdatedAt = DateTime.UtcNow;

            await _consignRepo.UpdateAsync(consignment);

            return (true, "Cập nhật hồ sơ ký gửi thành công!");
        }

        private static ConsignmentListItemDto MapToListItem(Consignment c) => new()
        {
            ConsignmentId = c.ConsignmentId,
            GuestName = c.GuestName,
            GuestPhone = c.GuestPhone,
            Brand = c.Brand,
            Model = c.Model,
            Year = c.Year,
            ExpectedPrice = c.ExpectedPrice,
            Status = c.Status,
            CreatedAt = c.CreatedAt.ToString("dd/MM/yyyy HH:mm")
        };

        private static ConsignmentResponseDto MapToResponse(Consignment c) => new()
        {
            ConsignmentId = c.ConsignmentId,
            GuestName = c.GuestName,
            GuestPhone = c.GuestPhone,
            GuestEmail = c.GuestEmail,
            Brand = c.Brand,
            Model = c.Model,
            Year = c.Year,
            Mileage = c.Mileage,
            ConditionDescription = c.ConditionDescription,
            ExpectedPrice = c.ExpectedPrice,
            AgreedPrice = c.AgreedPrice,
            CommissionRate = c.CommissionRate,
            Status = c.Status,
            LinkedCarId = c.LinkedCarId,
            CreatedAt = c.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
            UpdatedAt = c.UpdatedAt.ToString("dd/MM/yyyy HH:mm")
        };
    }
}