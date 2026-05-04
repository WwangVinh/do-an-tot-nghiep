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
    public class BookingAdminService : IBookingAdminService
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly INotificationService _notiService;

        public BookingAdminService(IBookingRepository bookingRepo, INotificationService notiService)
        {
            _bookingRepo = bookingRepo;
            _notiService = notiService;
        }

        private bool HasShowroomAccess(string userRole, int? userShowroomId, int? bookingShowroomId)
        {
            if (userRole == AppRoles.Admin) return true;
            if (userRole == AppRoles.Technician) return true; // Technician xem được tất cả để duyệt xe
            return userShowroomId.HasValue && bookingShowroomId == userShowroomId.Value;
        }

        private bool IsSalesSide(string userRole) =>
            userRole == AppRoles.Admin ||
            userRole == AppRoles.Manager ||
            userRole == AppRoles.Sales ||
            userRole == AppRoles.ShowroomSales;

        private string AppendNote(string? existingNote, string userRole, string content)
        {
            var line = $"[{DateTime.Now:dd/MM/yyyy HH:mm} - {userRole}]: {content.Trim()}";
            return string.IsNullOrWhiteSpace(existingNote) ? line : $"{existingNote}\n{line}";
        }

        public async Task<object> GetBookingsForAdminAsync(
            int page, int pageSize, string? search, string? status,
            string userRole, int? userShowroomId)
        {
            if (userRole != AppRoles.Admin && !userShowroomId.HasValue)
                return new { TotalCount = 0, Data = new List<object>() };

            int? filterShowroom = (userRole == AppRoles.Admin) ? null : userShowroomId;
            var (bookings, total) = await _bookingRepo.GetAdminBookingsAsync(
                page, pageSize, search, status, null, null, filterShowroom);

            return new
            {
                TotalCount = total,
                Data = bookings.Select(b => new
                {
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

        public async Task<object?> GetBookingDetailAsync(int bookingId, string userRole, int? userShowroomId)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null) return null;

            if (!HasShowroomAccess(userRole, userShowroomId, booking.ShowroomId))
                return null;

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

        public async Task<(bool Success, string Message)> MarkAsConsultedAsync(
            int bookingId, BookingConsultDto dto, string userRole, int? userShowroomId)
        {
            if (!IsSalesSide(userRole))
                return (false, "Chỉ Sales mới được thực hiện bước tư vấn.");

            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null) return (false, "Không tìm thấy lịch hẹn.");

            if (!HasShowroomAccess(userRole, userShowroomId, booking.ShowroomId))
                return (false, "Bạn không có quyền can thiệp vào lịch hẹn của chi nhánh khác!");

            if (booking.Status != BookingStatus.Pending)
                return (false, $"Lịch hẹn đang ở trạng thái '{booking.Status}', không thể chuyển sang tư vấn.");

            booking.Status = BookingStatus.Consulted;
            booking.Note = AppendNote(booking.Note, userRole, $"Kết quả tư vấn: {dto.ConsultNote}");
            booking.UpdatedAt = DateTime.Now;

            await _bookingRepo.UpdateAsync(booking);

            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: booking.ShowroomId,
                roleTarget: $"{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales}",
                title: "Lịch hẹn đã được tư vấn ✅",
                content: $"Lịch của khách {booking.CustomerName} ({booking.Phone}) - xe {booking.Car?.Name} đã được tư vấn xong. Cần kiểm tra xe và xác nhận lịch lái thử.",
                actionUrl: $"/bookings",
                type: "Booking"
            );

            return (true, "Đã ghi nhận kết quả tư vấn.");
        }

        public async Task<(bool Success, string Message)> SendToTechCheckAsync(
            int bookingId, BookingSendTechDto dto, string userRole, int? userShowroomId)
        {
            if (!IsSalesSide(userRole))
                return (false, "Chỉ Sales mới được gửi xe qua kỹ thuật.");

            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null) return (false, "Không tìm thấy lịch hẹn.");

            if (!HasShowroomAccess(userRole, userShowroomId, booking.ShowroomId))
                return (false, "Bạn không có quyền can thiệp vào lịch hẹn của chi nhánh khác!");

            if (booking.Status != BookingStatus.Consulted)
                return (false, $"Lịch hẹn đang ở trạng thái '{booking.Status}', cần tư vấn xong mới gửi kỹ thuật.");

            booking.Status = BookingStatus.PendingTechCheck;
            if (!string.IsNullOrWhiteSpace(dto.TechNote))
                booking.Note = AppendNote(booking.Note, userRole, $"Ghi chú gửi kỹ thuật: {dto.TechNote}");
            booking.UpdatedAt = DateTime.Now;

            await _bookingRepo.UpdateAsync(booking);

            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: booking.ShowroomId,
                roleTarget: AppRoles.Technician,
                title: "Xe cần kiểm tra kỹ thuật 🔧",
                content: $"Xe {booking.Car?.Name} cần kiểm tra trước lịch lái thử của khách {booking.CustomerName} vào {booking.BookingDate:dd/MM/yyyy} lúc {booking.BookingTime}.",
                actionUrl: $"/bookings",
                type: "TechCheck"
            );

            return (true, "Đã gửi yêu cầu kiểm tra kỹ thuật.");
        }

        public async Task<(bool Success, string Message)> SubmitTechResultAsync(
            int bookingId, BookingTechResultDto dto, string userRole, int? userShowroomId)
        {
            if (userRole != AppRoles.Technician && userRole != AppRoles.Admin)
                return (false, "Chỉ Kỹ thuật viên mới được cập nhật kết quả kiểm tra xe.");

            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null) return (false, "Không tìm thấy lịch hẹn.");

            if (!HasShowroomAccess(userRole, userShowroomId, booking.ShowroomId))
                return (false, "Bạn không có quyền can thiệp vào lịch hẹn của chi nhánh khác!");

            if (booking.Status != BookingStatus.PendingTechCheck)
                return (false, $"Lịch hẹn đang ở trạng thái '{booking.Status}', không phải đang chờ kiểm tra kỹ thuật.");

            if (dto.IsApproved)
            {
                booking.Status = BookingStatus.TechApproved;
                booking.Note = AppendNote(booking.Note, userRole, $"Kỹ thuật DUYỆT xe: {dto.TechNote}");
                booking.UpdatedAt = DateTime.Now;
                await _bookingRepo.UpdateAsync(booking);

                await _notiService.CreateNotificationAsync(
                    userId: null,
                    showroomId: booking.ShowroomId,
                    roleTarget: $"{AppRoles.Sales},{AppRoles.ShowroomSales},{AppRoles.Manager}",
                    title: "Xe đã sẵn sàng ✅",
                    content: $"Xe {booking.Car?.Name} đã qua kiểm tra kỹ thuật. Vui lòng xác nhận lịch lái thử với khách {booking.CustomerName}.",
                    actionUrl: $"/bookings",
                    type: "TechCheck"
                );

                return (true, "Xe đã được duyệt, đang chờ Sales xác nhận lịch với khách.");
            }
            else
            {
                booking.Status = BookingStatus.Consulted;
                booking.Note = AppendNote(booking.Note, userRole, $"Kỹ thuật TỪ CHỐI xe: {dto.TechNote}");
                booking.UpdatedAt = DateTime.Now;
                await _bookingRepo.UpdateAsync(booking);

                await _notiService.CreateNotificationAsync(
                    userId: null,
                    showroomId: booking.ShowroomId,
                    roleTarget: $"{AppRoles.Sales},{AppRoles.ShowroomSales},{AppRoles.Manager}",
                    title: "Xe chưa đạt kỹ thuật ⚠️",
                    content: $"Xe {booking.Car?.Name} chưa đạt kiểm tra kỹ thuật. Lý do: {dto.TechNote}. Vui lòng liên hệ khách {booking.CustomerName} để xử lý.",
                    actionUrl: $"/bookings",
                    type: "TechCheck"
                );

                return (true, "Đã ghi nhận xe chưa đạt, lịch hẹn trả về cho Sales xử lý.");
            }
        }

        public async Task<(bool Success, string Message)> ConfirmBookingAsync(
            int bookingId, string userRole, int? userShowroomId)
        {
            if (!IsSalesSide(userRole))
                return (false, "Chỉ Sales mới được xác nhận lịch hẹn.");

            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null) return (false, "Không tìm thấy lịch hẹn.");

            if (!HasShowroomAccess(userRole, userShowroomId, booking.ShowroomId))
                return (false, "Bạn không có quyền can thiệp vào lịch hẹn của chi nhánh khác!");

            if (booking.Status != BookingStatus.TechApproved)
                return (false, $"Lịch hẹn đang ở trạng thái '{booking.Status}', xe cần được kỹ thuật duyệt trước.");

            booking.Status = BookingStatus.Confirmed;
            booking.Note = AppendNote(booking.Note, userRole, "Đã xác nhận lịch lái thử với khách.");
            booking.UpdatedAt = DateTime.Now;

            await _bookingRepo.UpdateAsync(booking);

            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: booking.ShowroomId,
                roleTarget: $"{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales}",
                title: "Lịch lái thử đã được xác nhận 🎉",
                content: $"Lịch lái thử xe {booking.Car?.Name} của khách {booking.CustomerName} ({booking.Phone}) đã xác nhận vào {booking.BookingDate:dd/MM/yyyy} lúc {booking.BookingTime}.",
                actionUrl: $"/bookings",
                type: "Booking"
            );

            return (true, "Đã xác nhận lịch hẹn thành công.");
        }

        public async Task<(bool Success, string Message)> CompleteBookingAsync(
            int bookingId, string? resultNote, string userRole, int? userShowroomId)
        {
            if (!IsSalesSide(userRole))
                return (false, "Bạn không có quyền thực hiện thao tác này.");

            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null) return (false, "Không tìm thấy lịch hẹn.");

            if (!HasShowroomAccess(userRole, userShowroomId, booking.ShowroomId))
                return (false, "Bạn không có quyền can thiệp vào lịch hẹn của chi nhánh khác!");

            if (booking.Status != BookingStatus.Confirmed)
                return (false, $"Lịch hẹn đang ở trạng thái '{booking.Status}', cần Confirmed mới hoàn thành được.");

            booking.Status = BookingStatus.Completed;
            if (!string.IsNullOrWhiteSpace(resultNote))
                booking.Note = AppendNote(booking.Note, userRole, $"Kết quả lái thử: {resultNote}");
            booking.UpdatedAt = DateTime.Now;

            await _bookingRepo.UpdateAsync(booking);
            return (true, "Đã hoàn thành lịch hẹn.");
        }

        public async Task<(bool Success, string Message)> MarkNoShowAsync(
            int bookingId, BookingNoShowDto dto, string userRole, int? userShowroomId)
        {
            if (!IsSalesSide(userRole))
                return (false, "Bạn không có quyền thực hiện thao tác này.");

            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null) return (false, "Không tìm thấy lịch hẹn.");

            if (!HasShowroomAccess(userRole, userShowroomId, booking.ShowroomId))
                return (false, "Bạn không có quyền can thiệp vào lịch hẹn của chi nhánh khác!");

            if (booking.Status != BookingStatus.Confirmed)
                return (false, $"Lịch hẹn đang ở trạng thái '{booking.Status}', chỉ báo NoShow được khi đã Confirmed.");

            booking.Status = BookingStatus.NoShow;
            var reason = string.IsNullOrWhiteSpace(dto.Reason) ? "Khách không đến đúng hẹn" : dto.Reason;
            booking.Note = AppendNote(booking.Note, userRole, $"NoShow: {reason}");
            booking.UpdatedAt = DateTime.Now;

            await _bookingRepo.UpdateAsync(booking);

            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: booking.ShowroomId,
                roleTarget: AppRoles.Manager,
                title: "Khách không đến đúng hẹn",
                content: $"Khách {booking.CustomerName} ({booking.Phone}) không đến lái thử xe {booking.Car?.Name} vào {booking.BookingDate:dd/MM/yyyy} lúc {booking.BookingTime}.",
                actionUrl: $"/bookings",
                type: "SystemAlert"
            );

            return (true, "Đã ghi nhận khách không tới.");
        }

        public async Task<(bool Success, string Message)> CancelBookingByAdminAsync(
            int bookingId, BookingCancelDto dto, string userRole, int? userShowroomId)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null) return (false, "Không tìm thấy lịch hẹn.");

            if (!HasShowroomAccess(userRole, userShowroomId, booking.ShowroomId))
                return (false, "Bạn không có quyền hủy lịch của chi nhánh khác!");

            if (booking.Status == BookingStatus.Completed)
                return (false, "Lịch hẹn đã hoàn thành, không thể hủy.");

            if (booking.Status == BookingStatus.Cancelled)
                return (false, "Lịch hẹn này đã bị hủy trước đó rồi.");

            booking.Status = BookingStatus.Cancelled;
            booking.Note = AppendNote(booking.Note, userRole, $"Hủy lịch: {dto.CancelReason}");
            booking.UpdatedAt = DateTime.Now;

            await _bookingRepo.UpdateAsync(booking);

            if (userRole != AppRoles.Manager && userRole != AppRoles.Admin)
            {
                await _notiService.CreateNotificationAsync(
                    userId: null,
                    showroomId: booking.ShowroomId,
                    roleTarget: AppRoles.Manager,
                    title: "Cảnh báo: Lịch hẹn bị hủy",
                    content: $"Nhân viên [{userRole}] vừa hủy lịch hẹn của khách {booking.CustomerName}. Lý do: {dto.CancelReason}",
                    actionUrl: $"/bookings",
                    type: "SystemAlert"
                );
            }

            return (true, "Đã hủy lịch hẹn thành công.");
        }

        public async Task<object> GetPendingTechCheckAsync(string userRole, int? userShowroomId)
        {
            if (userRole != AppRoles.Technician && userRole != AppRoles.Admin && userRole != AppRoles.Manager)
                return new List<object>();

            int? filterShowroom = (userRole == AppRoles.Admin) ? null : userShowroomId;
            var bookings = await _bookingRepo.GetPendingTechCheckAsync(filterShowroom);

            return bookings.Select(b => new
            {
                b.BookingId,
                b.CustomerName,
                b.Phone,
                b.BookingDate,
                b.BookingTime,
                CarName = b.Car?.Name,
                ShowroomName = b.Showroom?.Name,
                b.Note
            });
        }

        public async Task<Dictionary<string, int>> GetBookingStatsAsync(string userRole, int? userShowroomId)
        {
            int? filterShowroom = (userRole == AppRoles.Admin) ? null : userShowroomId;
            return await _bookingRepo.CountByStatusAsync(filterShowroom);
        }
    }
}