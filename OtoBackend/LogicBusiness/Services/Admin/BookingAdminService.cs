using CoreEntities.Models;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Interfaces.Shared;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LogicBusiness.Services.Admin
{
    public class BookingAdminService : IBookingAdminService
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly INotificationService _notiService;

        public BookingAdminService(IBookingRepository bookingRepo, INotificationService notiService)
        {
            _bookingRepo = bookingRepo;
            _notiService = notiService;
        }

        // LẤY DANH SÁCH
        public async Task<object> GetBookingsForAdminAsync(int page, int pageSize, string? search, string? status, string userRole, int? userShowroomId)
        {
            // Nếu không phải Admin mà cũng không có ShowroomId -> Chặn
            if (userRole != "Admin" && !userShowroomId.HasValue)
            {
                return new { TotalCount = 0, Data = new List<object>() };
            }

            int? filterShowroom = (userRole == "Admin") ? null : userShowroomId;

            var (bookings, total) = await _bookingRepo.GetAdminBookingsAsync(page, pageSize, search, status, null, null, filterShowroom);

            return new
            {
                TotalCount = total,
                Data = bookings.Select(b => new {
                    b.BookingId,
                    b.CustomerName,
                    b.Phone,
                    b.BookingDate,
                    b.BookingTime,
                    b.Status,
                    CarName = b.Car?.Name,
                    ShowroomName = b.Showroom?.Name,
                    CreatedAt = b.CreatedAt?.ToString("dd/MM/yyyy HH:mm")
                })
            };
        }
        // XEM CHI TIẾT
        public async Task<object?> GetBookingDetailAsync(int bookingId, string userRole, int? userShowroomId)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null) return null;

            // Manager/Sales chi nhánh này không xem được chi nhánh kia
            if (userRole != "Admin")
            {
                if (!userShowroomId.HasValue || booking.ShowroomId != userShowroomId.Value)
                    return null;
            }

            return new
            {
                booking.BookingId,
                booking.CustomerName,
                booking.Phone,
                booking.BookingDate,
                booking.BookingTime,
                booking.Status,
                booking.Note,
                CarDetails = new { booking.Car?.CarId, booking.Car?.Name, ImageUrl = booking.Car?.ImageUrl },
                ShowroomDetails = new { booking.Showroom?.ShowroomId, booking.Showroom?.Name, booking.Showroom?.Province },
                booking.CreatedAt,
                booking.UpdatedAt
            };
        }

        // ĐỔI TRẠNG THÁI
        public async Task<(bool Success, string Message)> UpdateBookingStatusAsync(int bookingId, string newStatus, string userRole, int? userShowroomId)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null) return (false, "Không tìm thấy lịch hẹn.");

            if (userRole != "Admin")
            {
                if (!userShowroomId.HasValue || booking.ShowroomId != userShowroomId.Value)
                    return (false, "Bạn không có quyền can thiệp vào lịch hẹn của chi nhánh khác!");
            }

            if (booking.Status == "Cancelled" || booking.Status == "Completed")
                return (false, "Lịch hẹn này đã kết thúc, không thể thay đổi trạng thái.");

            booking.Status = newStatus;
            booking.UpdatedAt = DateTime.Now;

            await _bookingRepo.UpdateAsync(booking);

            if (booking.UserId.HasValue)
            {
                string statusVN = newStatus == "Confirmed" ? "Đã được xác nhận" : newStatus == "Completed" ? "Đã hoàn thành" : newStatus;
                await _notiService.CreateNotificationAsync(
                    userId: booking.UserId.Value, 
                    showroomId: null,
                    roleTarget: null,
                    title: "Cập nhật lịch hẹn lái thử 📅",
                    content: $"Lịch hẹn xem xe của bạn {statusVN}. Vui lòng kiểm tra chi tiết!",
                    actionUrl: $"/customer/my-bookings/{bookingId}",
                    type: "Booking"
                );
            }
            return (true, $"[{userRole}] Đã cập nhật trạng thái thành: {newStatus}");
        }

        // Cập nhật trạng thái + kết quả (kết quả lưu vào Note dạng log)
        public async Task<(bool Success, string Message)> UpdateBookingAsync(int bookingId, string? newStatus, string? result, string? userRole, int? userShowroomId)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null) return (false, "Không tìm thấy lịch hẹn.");

            string role = string.IsNullOrWhiteSpace(userRole) ? "Staff" : userRole!;

            if (role != "Admin")
            {
                if (!userShowroomId.HasValue || booking.ShowroomId != userShowroomId.Value)
                    return (false, "Ní không có quyền can thiệp vào lịch hẹn của chi nhánh khác!");
            }

            if (booking.Status == "Cancelled")
                return (false, "Lịch hẹn này đã bị hủy, không thể cập nhật.");

            bool changed = false;

            if (!string.IsNullOrWhiteSpace(newStatus))
            {
                // Dùng lại rule của UpdateBookingStatusAsync (chặn đổi khi đã Completed)
                if (booking.Status == "Completed")
                    return (false, "Lịch hẹn này đã hoàn thành, không thể thay đổi trạng thái.");

                booking.Status = newStatus!;
                changed = true;
            }

            if (!string.IsNullOrWhiteSpace(result))
            {
                var timeStamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                var line = $"[{timeStamp} - {role} cập nhật kết quả]: {result!.Trim()}";
                booking.Note = string.IsNullOrWhiteSpace(booking.Note) ? line : $"{booking.Note}\n{line}";
                changed = true;
            }

            if (!changed) return (false, "Không có dữ liệu cập nhật.");

            booking.UpdatedAt = DateTime.Now;
            await _bookingRepo.UpdateAsync(booking);

            if (booking.UserId.HasValue)
            {
                // Thông báo ngắn gọn cho khách (tránh lộ nội dung kết quả)
                await _notiService.CreateNotificationAsync(
                    userId: booking.UserId.Value,
                    showroomId: null,
                    roleTarget: null,
                    title: "Cập nhật lịch hẹn 📅",
                    content: "Lịch hẹn của bạn vừa được Showroom cập nhật. Vui lòng kiểm tra chi tiết!",
                    actionUrl: $"/customer/my-bookings/{bookingId}",
                    type: "Booking"
                );
            }

            return (true, "Đã cập nhật lịch hẹn.");
        }

        // HỦY LỊCH (Admin/Manager/Sales hủy khi khách bom)
        public async Task<(bool Success, string Message)> CancelBookingByAdminAsync(int bookingId, string cancelReason, string userRole, int? userShowroomId)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null) return (false, "Không tìm thấy lịch hẹn.");

            if (userRole != "Admin" && booking.ShowroomId != userShowroomId)
                return (false, "Ní không có quyền hủy lịch của chi nhánh khác!");

            if (string.IsNullOrWhiteSpace(cancelReason))
                return (false, "Vui lòng nhập lý do hủy để báo cáo sếp nhé!");

            booking.Status = "Cancelled";
            booking.UpdatedAt = DateTime.Now;

            // Log lý do hủy
            string timeStamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            booking.Note = string.IsNullOrWhiteSpace(booking.Note)
                ? $"[{timeStamp} - {userRole} hủy]: {cancelReason}"
                : $"{booking.Note}\n[{timeStamp} - {userRole} hủy]: {cancelReason}";

            await _bookingRepo.UpdateAsync(booking);
            if (booking.UserId.HasValue)
            {
                await _notiService.CreateNotificationAsync(
                    userId: booking.UserId.Value,
                    showroomId: null,
                    roleTarget: null,
                    title: "Lịch hẹn đã bị hủy ❌",
                    content: $"Lịch hẹn của bạn đã bị hủy bởi Showroom. Lý do: {cancelReason}",
                    actionUrl: $"/customer/my-bookings/{bookingId}",
                    type: "Booking"
                );
            }

            // THÊM MỚI: Báo cho Manager của Showroom đó biết
            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: booking.ShowroomId, // Target vào đúng chi nhánh đó
                roleTarget: "Manager",         // Chỉ báo cho Manager
                title: "Cảnh báo hủy lịch hẹn",
                content: $"Nhân viên {userRole} vừa hủy lịch hẹn của khách {booking.CustomerName}. Lý do: {cancelReason}",
                actionUrl: $"/admin/bookings/{bookingId}",
                type: "SystemAlert"
            );
            return (true, "Đã hủy lịch hẹn thành công.");
        }
    }
}