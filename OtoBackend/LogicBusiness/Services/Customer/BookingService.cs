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
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly ICarInventoryRepository _inventoryRepo;
        private readonly ICarRepository _carRepo;
        private readonly INotificationService _notiService;

        public BookingService(
            IBookingRepository bookingRepo,
            ICarInventoryRepository inventoryRepo,
            ICarRepository carRepo,
            INotificationService notiService)
        {
            _bookingRepo = bookingRepo;
            _inventoryRepo = inventoryRepo;
            _carRepo = carRepo;
            _notiService = notiService;
        }

        public async Task<(bool Success, string Message)> CreateBookingAsync(BookingCreateDto dto)
        {
            var car = await _carRepo.GetByIdAsync(dto.CarId);
            if (car == null)
                return (false, "Xe không tồn tại!");

            if (car.Status != CarStatus.Available && car.Status != CarStatus.COMING_SOON)
                return (false, "Rất tiếc, xe này hiện đã có người cọc hoặc ngừng giao dịch.");

            var inventory = await _inventoryRepo.GetInventoryAsync(dto.CarId, dto.ShowroomId);
            if (inventory == null || inventory.Quantity <= 0)
                return (false, "Chi nhánh này hiện không có sẵn mẫu xe bạn chọn. Bạn thử chọn cơ sở khác xem!");

            bool isTimeSlotTaken = await _bookingRepo.IsTimeSlotBookedAsync(
                dto.CarId, dto.ShowroomId, dto.BookingDate, dto.BookingTime);
            if (isTimeSlotTaken)
                return (false, "Khung giờ này đã có khách khác hẹn xem xe rồi. Bạn vui lòng chọn giờ khác nhé!");

            var booking = new Booking
            {
                CarId = dto.CarId,
                ShowroomId = dto.ShowroomId,
                CustomerName = dto.CustomerName.Trim(),
                Phone = dto.Phone.Trim(),
                BookingDate = dto.BookingDate,
                BookingTime = dto.BookingTime,
                Note = dto.Note,
                UserId = null,
                Status = BookingStatus.Pending,
                CreatedAt = DateTime.Now
            };

            await _bookingRepo.AddAsync(booking);

            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: dto.ShowroomId,
                roleTarget: $"{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales}",
                title: "Có lịch hẹn lái thử mới! 📅",
                content: $"Khách {dto.CustomerName} ({dto.Phone}) vừa đặt lịch lái thử {car.Brand} {car.Name} lúc {dto.BookingTime} ngày {dto.BookingDate:dd/MM/yyyy}.",
                actionUrl: "/bookings",
                type: "Booking"
            );

            return (true, "Đặt lịch hẹn lái thử thành công! Nhân viên sẽ liên hệ bạn sớm nhé.");
        }

        public async Task<IEnumerable<object>> GetBookingsByPhoneAsync(string phone)
        {
            var bookings = await _bookingRepo.GetByPhoneAsync(phone);

            return bookings.Select(b => new
            {
                b.BookingId,
                b.BookingDate,
                b.BookingTime,
                b.Status,
                StatusLabel = ToStatusLabel(b.Status),
                CarName = b.Car?.Name,
                CarImage = b.Car?.ImageUrl,
                ShowroomName = b.Showroom?.Name,
                CreatedAt = b.CreatedAt?.ToString("dd/MM/yyyy HH:mm")
            });
        }

        public async Task<object?> GetBookingDetailByPhoneAsync(int bookingId, string phone)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);

            if (booking == null || booking.Phone != phone)
                return null;

            return new
            {
                booking.BookingId,
                booking.CustomerName,
                booking.Phone,
                booking.BookingDate,
                booking.BookingTime,
                booking.Status,
                StatusLabel = ToStatusLabel(booking.Status),
                HasNote = !string.IsNullOrWhiteSpace(booking.Note),
                CarDetails = new
                {
                    booking.Car?.CarId,
                    booking.Car?.Name,
                    booking.Car?.Brand,
                    ImageUrl = booking.Car?.ImageUrl
                },
                ShowroomDetails = new
                {
                    booking.Showroom?.ShowroomId,
                    booking.Showroom?.Name,
                    booking.Showroom?.District,
                    booking.Showroom?.StreetAddress
                },
                CreatedAt = booking.CreatedAt?.ToString("dd/MM/yyyy HH:mm"),
                UpdatedAt = booking.UpdatedAt?.ToString("dd/MM/yyyy HH:mm"),
                Timeline = BuildTimeline(booking.Status)
            };
        }

        public async Task<(bool Success, string Message)> CancelBookingByPhoneAsync(int bookingId, string phone, string? reason)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);

            if (booking == null || booking.Phone != phone)
                return (false, "Không tìm thấy lịch hẹn hoặc số điện thoại không khớp.");

            if (booking.Status == BookingStatus.PendingTechCheck ||
                booking.Status == BookingStatus.TechApproved ||
                booking.Status == BookingStatus.Confirmed)
                return (false, "Lịch hẹn đang trong quá trình chuẩn bị, bạn vui lòng liên hệ trực tiếp showroom để hủy nhé!");

            if (booking.Status == BookingStatus.Completed)
                return (false, "Lịch hẹn này đã hoàn thành, không thể hủy.");

            if (booking.Status == BookingStatus.Cancelled)
                return (false, "Lịch hẹn này đã bị hủy trước đó rồi.");

            booking.Status = BookingStatus.Cancelled;
            booking.UpdatedAt = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(reason))
            {
                var line = $"[{DateTime.Now:dd/MM/yyyy HH:mm} - Khách hủy]: {reason.Trim()}";
                booking.Note = string.IsNullOrWhiteSpace(booking.Note)
                    ? line
                    : $"{booking.Note}\n{line}";
            }

            await _bookingRepo.UpdateAsync(booking);

            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: booking.ShowroomId,
                roleTarget: $"{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales}",
                title: "Khách tự hủy lịch hẹn ❌",
                content: $"Khách {booking.CustomerName} ({booking.Phone}) vừa tự hủy lịch lái thử xe {booking.Car?.Name}."
                    + (string.IsNullOrWhiteSpace(reason) ? "" : $" Lý do: {reason}"),
                actionUrl: "/bookings",
                type: "Booking"
            );

            return (true, "Đã hủy lịch hẹn thành công!");
        }

        private static string ToStatusLabel(string? status) => status switch
        {
            BookingStatus.Pending => "Chờ tư vấn",
            BookingStatus.Consulted => "Đã tư vấn",
            BookingStatus.PendingTechCheck => "Đang kiểm tra xe",
            BookingStatus.TechApproved => "Xe sẵn sàng",
            BookingStatus.Confirmed => "Đã xác nhận lịch",
            BookingStatus.Completed => "Hoàn thành",
            BookingStatus.NoShow => "Không đến",
            BookingStatus.Cancelled => "Đã hủy",
            _ => status ?? "Không xác định"
        };

        private static object BuildTimeline(string? currentStatus)
        {
            var steps = new[]
            {
                BookingStatus.Pending,
                BookingStatus.Consulted,
                BookingStatus.PendingTechCheck,
                BookingStatus.TechApproved,
                BookingStatus.Confirmed,
                BookingStatus.Completed
            };

            if (currentStatus == BookingStatus.Cancelled || currentStatus == BookingStatus.NoShow)
                return new { IsTerminated = true, Status = currentStatus };

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