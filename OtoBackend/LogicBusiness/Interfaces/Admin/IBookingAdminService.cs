using CoreEntities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Admin
{
    public interface IBookingAdminService
    {
        // Lấy danh sách lịch hẹn cho Admin/Manager/Sales
        // Phân quyền: Manager/Sales chỉ thấy showroom mình, Admin thấy tất cả.
        Task<object> GetBookingsForAdminAsync(
            int page,
            int pageSize,
            string? search,
            string? status,
            string userRole,
            int? userShowroomId);

        // Xem chi tiết lịch hẹn (Có check quyền showroom)
        Task<object?> GetBookingDetailAsync(
            int bookingId,
            string userRole,
            int? userShowroomId);

        // Cập nhật trạng thái (Dùng cho cả 3 giai cấp - Có check quyền chéo)
        // Vd: Scheduled -> Completed (Sau khi khách lái xong)
        Task<(bool Success, string Message)> UpdateBookingStatusAsync(
            int bookingId,
            string newStatus,
            string userRole,
            int? userShowroomId);

        // Cập nhật trạng thái + kết quả (kết quả lưu vào Note dạng log)
        Task<(bool Success, string Message)> UpdateBookingAsync(
            int bookingId,
            string? newStatus,
            string? result,
            string? userRole,
            int? userShowroomId);

        // Hủy lịch hẹn từ phía nội bộ (Dành cho Manager/Sales khi khách "bom")
        Task<(bool Success, string Message)> CancelBookingByAdminAsync(
            int bookingId,
            string cancelReason,
            string userRole,
            int? userShowroomId);
    }
}