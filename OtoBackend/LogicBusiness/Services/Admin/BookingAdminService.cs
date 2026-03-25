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

        // 1. LẤY DANH SÁCH (Chặn lỗ hổng filter null)
        public async Task<object> GetBookingsForAdminAsync(int page, int pageSize, string? search, string? status, string userRole, int? userShowroomId)
        {
            // BẢO MẬT: Nếu không phải Admin mà cũng không có ShowroomId -> Chặn luôn không cho lấy gì hết
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
        // 2. XEM CHI TIẾT (Check quyền lấn sân chi nhánh)
        public async Task<object?> GetBookingDetailAsync(int bookingId, string userRole, int? userShowroomId)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null) return null;

            // Chặn chéo: Manager/Sales chi nhánh này không xem được chi nhánh kia
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

        // 3. ĐỔI TRẠNG THÁI (Check quyền gắt gao)
        public async Task<(bool Success, string Message)> UpdateBookingStatusAsync(int bookingId, string newStatus, string userRole, int? userShowroomId)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null) return (false, "Không tìm thấy lịch hẹn.");

            // CHẶN CHÉO TUYỆT ĐỐI
            if (userRole != "Admin")
            {
                if (!userShowroomId.HasValue || booking.ShowroomId != userShowroomId.Value)
                    return (false, "Ní không có quyền can thiệp vào lịch hẹn của chi nhánh khác!");
            }

            // Chặn đổi trạng thái nếu lịch đã đóng
            if (booking.Status == "Cancelled" || booking.Status == "Completed")
                return (false, "Lịch hẹn này đã kết thúc, không thể thay đổi trạng thái.");

            booking.Status = newStatus;
            booking.UpdatedAt = DateTime.Now;

            await _bookingRepo.UpdateAsync(booking);

            if (booking.UserId.HasValue)
            {
                string statusVN = newStatus == "Confirmed" ? "Đã được xác nhận" : newStatus == "Completed" ? "Đã hoàn thành" : newStatus;
                await _notiService.CreateNotificationAsync(
                    userId: booking.UserId.Value, // Bắn đích danh vào tài khoản của khách
                    showroomId: null,
                    title: "Cập nhật lịch hẹn lái thử 📅",
                    content: $"Lịch hẹn xem xe của bạn {statusVN}. Vui lòng kiểm tra chi tiết!",
                    actionUrl: $"/customer/my-bookings/{bookingId}", // Link trỏ khách về trang quản lý lịch của họ
                    type: "Booking"
                );
            }
            return (true, $"[{userRole}] Đã cập nhật trạng thái thành: {newStatus}");
        }

        // 4. HỦY LỊCH (Admin/Manager/Sales hủy khi khách bom)
        public async Task<(bool Success, string Message)> CancelBookingByAdminAsync(int bookingId, string cancelReason, string userRole, int? userShowroomId)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null) return (false, "Không tìm thấy lịch hẹn.");

            // Kiểm tra quyền showroom
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
                    title: "Lịch hẹn đã bị hủy ❌",
                    content: $"Lịch hẹn của bạn đã bị hủy bởi Showroom. Lý do: {cancelReason}",
                    actionUrl: $"/customer/my-bookings/{bookingId}",
                    type: "Booking"
                );
            }
            return (true, "Đã hủy lịch hẹn thành công.");
        }
    }
}