using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Customer;
using LogicBusiness.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Services.Customer
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepo; // Ní tự tạo Repo này nhé
        private readonly ICarInventoryRepository _inventoryRepo;
        private readonly ICarRepository _carRepo;

        public BookingService(IBookingRepository bookingRepo, ICarInventoryRepository inventoryRepo, ICarRepository carRepo)
        {
            _bookingRepo = bookingRepo;
            _inventoryRepo = inventoryRepo;
            _carRepo = carRepo;
        }

        public async Task<(bool Success, string Message)> CreateBookingAsync(BookingCreateDto dto)
        {
            // 1. Kiểm tra xe còn trên sàn không?
            var car = await _carRepo.GetByIdAsync(dto.CarId);
            if (car == null)
                return (false, "Xe không tồn tại!");

            if (car.Status != CarStatus.Available && car.Status != CarStatus.COMING_SOON)
                return (false, "Rất tiếc, xe này hiện đã có người cọc hoặc ngừng giao dịch.");

            // 2. Kiểm tra xem xe có đang ở chi nhánh khách muốn đến xem không?
            var inventory = await _inventoryRepo.GetInventoryAsync(dto.CarId, dto.ShowroomId);
            if (inventory == null || inventory.Quantity <= 0)
                return (false, "Chi nhánh này hiện không có sẵn mẫu xe bạn chọn. Ní thử chọn cơ sở khác xem!");

            // 3. BÍ KÍP CHỐNG ĐỤNG ĐỘ: Kiểm tra xem có ai hẹn xem con xe ĐỘC BẢN này cùng giờ không?
            // Giả sử 1 ca lái thử / tư vấn tốn khoảng 2 tiếng (Ní phải viết thêm hàm này trong BookingRepo nhé)
            bool isTimeSlotTaken = await _bookingRepo.IsTimeSlotBookedAsync(dto.CarId, dto.ShowroomId, dto.BookingDate, dto.BookingTime);
            if (isTimeSlotTaken)
            {
                return (false, "Khung giờ này đã có khách VIP khác hẹn xem xe rồi. Ní vui lòng chọn giờ khác nhé!");
            }

            // 4. LÊN LỊCH HẸN TRẢI NGHIỆM (Tuyệt đối KHÔNG trừ kho)
            var booking = new Booking
            {
                CarId = dto.CarId,
                ShowroomId = dto.ShowroomId,
                CustomerName = dto.CustomerName,
                Phone = dto.Phone,
                BookingDate = dto.BookingDate,
                BookingTime = dto.BookingTime,
                Note = dto.Note,
                UserId = dto.UserId,
                Status = "Scheduled", // Trạng thái: Đã lên lịch hẹn xem xe
                CreatedAt = DateTime.Now
            };

            await _bookingRepo.AddAsync(booking);

            return (true, "Đặt lịch hẹn lái thử thành công! Sale bên em sẽ liên hệ để đón ní nhé.");
        }

        // Nhớ thêm 2 hàm này vào IBookingService.cs nữa nha ní
        public async Task<IEnumerable<object>> GetMyBookingsAsync(int userId)
        {
            var bookings = await _bookingRepo.GetByUserIdAsync(userId); // Ní viết hàm này trong Repo nha
            return bookings.Select(b => new {
                b.BookingId,
                b.BookingDate,
                b.BookingTime,
                b.Status,
                CarName = b.Car?.Name,
                ShowroomName = b.Showroom?.Name,
                b.CreatedAt
            });
        }

        public async Task<(bool Success, string Message)> CancelBookingAsync(int bookingId, int userId)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null || booking.UserId != userId)
                return (false, "Không tìm thấy lịch hẹn hoặc ní không có quyền hủy lịch này!");

            if (booking.Status != "Scheduled")
                return (false, "Lịch hẹn này không ở trạng thái có thể hủy.");

            booking.Status = "Cancelled";
            booking.UpdatedAt = DateTime.Now;

            await _bookingRepo.UpdateAsync(booking);
            return (true, "Đã hủy lịch hẹn thành công!");
        }
    }
}
