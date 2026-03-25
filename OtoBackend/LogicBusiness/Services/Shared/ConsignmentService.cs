using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Interfaces.Shared; // Nhớ có cái này để dùng Chuông thông báo
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

        public ConsignmentService(IConsignmentRepository consignRepo, INotificationService notiService)
        {
            _consignRepo = consignRepo;
            _notiService = notiService;
        }

        // 1. KHÁCH HÀNG TẠO YÊU CẦU KÝ GỬI
        public async Task<(bool Success, string Message)> CreateConsignmentAsync(int userId, string customerName, ConsignmentCreateDto dto)
        {
            var consignment = new Consignment
            {
                UserId = userId,
                Brand = dto.Brand.Trim(),
                Model = dto.Model.Trim(),
                Year = dto.Year,
                Mileage = dto.Mileage,
                ConditionDescription = dto.ConditionDescription,
                ExpectedPrice = dto.ExpectedPrice,
                Status = "Pending", // Mới tạo là Chờ xử lý
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _consignRepo.AddAsync(consignment);

            // 👇 BẮN THÔNG BÁO CHO ADMIN/MANAGER BIẾT CÓ KÈO THƠM 👇
            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: null, // Gửi cho dàn Quản lý tổng / Sales để vô giành khách
                title: "Yêu cầu ký gửi xe mới! 🚗",
                content: $"Khách hàng {customerName} muốn ký gửi chiếc {dto.Brand} {dto.Model} ({dto.Year}). Sếp vào check ngay!",
                actionUrl: "/admin/consignments",
                type: "System"
            );

            return (true, "Gửi yêu cầu ký gửi thành công! Showroom sẽ liên hệ bạn sớm nhất để thẩm định xe.");
        }

        // 2. SẾP / ADMIN CẬP NHẬT TRẠNG THÁI & CHỐT GIÁ
        public async Task<(bool Success, string Message)> UpdateConsignmentStatusAsync(int consignmentId, ConsignmentUpdateDto dto, string adminRole)
        {
            var consignment = await _consignRepo.GetByIdAsync(consignmentId);
            if (consignment == null) return (false, "Không tìm thấy hồ sơ ký gửi này.");

            // Chỉ Admin hoặc Manager mới được sửa giá/hoa hồng
            if (adminRole != "Admin" && adminRole != "ShowroomManager")
            {
                return (false, "Chỉ quản lý mới được quyền chốt giá và cập nhật hồ sơ ký gửi!");
            }

            consignment.Status = dto.Status;
            if (dto.AgreedPrice.HasValue) consignment.AgreedPrice = dto.AgreedPrice.Value;
            if (dto.CommissionRate.HasValue) consignment.CommissionRate = dto.CommissionRate.Value;
            if (dto.LinkedCarId.HasValue) consignment.LinkedCarId = dto.LinkedCarId.Value;

            consignment.UpdatedAt = DateTime.Now;

            await _consignRepo.UpdateAsync(consignment);

            // 👇 BẮN THÔNG BÁO CHO KHÁCH HÀNG KHI HỒ SƠ ĐƯỢC CẬP NHẬT 👇
            if (consignment.UserId > 0)
            {
                string statusVN = dto.Status == "Appraising" ? "đang được thẩm định"
                                : dto.Status == "Approved" ? "đã được phê duyệt"
                                : dto.Status == "Rejected" ? "bị từ chối"
                                : dto.Status;

                await _notiService.CreateNotificationAsync(
                    userId: consignment.UserId, // Gắn đích danh mã khách hàng
                    showroomId: null,
                    title: "Cập nhật hồ sơ ký gửi 📝",
                    content: $"Hồ sơ ký gửi xe {consignment.Brand} {consignment.Model} của bạn {statusVN}. Vui lòng kiểm tra chi tiết!",
                    actionUrl: "/customer/my-consignments",
                    type: "System"
                );
            }

            return (true, "Cập nhật hồ sơ ký gửi thành công!");
        }
    }
}