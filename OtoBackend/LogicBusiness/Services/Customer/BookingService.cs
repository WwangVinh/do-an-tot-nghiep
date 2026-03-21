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
            // 1. Kiểm tra xe có tồn tại và có được phép bán không?
            var car = await _carRepo.GetByIdAsync(dto.CarId);
            if (car == null)
                return (false, "Xe không tồn tại!");

            if (car.Status != CarStatus.Available && car.Status != CarStatus.COMING_SOON)
                return (false, "Rất tiếc, xe này hiện không được mở bán.");

            // 2. BƯỚC TỬ THẦN: Kiểm tra kho tại chi nhánh khách chọn
            var inventory = await _inventoryRepo.GetInventoryAsync(dto.CarId, dto.ShowroomId);

            if (inventory == null || inventory.Quantity <= 0)
                return (false, "Chậm mất rồi ní ơi! Xe ở cơ sở này vừa có người đặt cọc hoặc đã hết. Vui lòng chọn cơ sở khác!");

            // 3. Vượt qua cửa ải -> Khách nạp tiền cọc thành công -> Lên Đơn!
            var booking = new Booking
            {
                CarId = dto.CarId,
                ShowroomId = dto.ShowroomId, // Giờ thì đã biết trừ ở đâu
                CustomerName = dto.CustomerName,
                Phone = dto.Phone,
                BookingDate = dto.BookingDate,
                BookingTime = dto.BookingTime,
                Note = dto.Note,
                UserId = dto.UserId,
                Status = "Deposited", // Trạng thái: Đã đặt cọc (Chắc ăn 100%)
                CreatedAt = DateTime.Now
            };

            // 4. TRỪ KHO NGAY LẬP TỨC (Giữ chỗ)
            inventory.Quantity -= 1;

            // Nếu kho tụt về 0, tự động treo biển hết hàng ở cơ sở đó
            if (inventory.Quantity == 0)
            {
                inventory.DisplayStatus = "Out of stock";
            }

            // 5. CHỐT LƯU VÀO DATABASE
            await _bookingRepo.AddAsync(booking);
            await _inventoryRepo.UpdateInventoryAsync(inventory);

            // Tùy chọn: Gọi cái hàm SyncCarStatusAsync (như bên Admin) để check xem 
            // nếu TẤT CẢ các cơ sở đều kho = 0 thì ép trạng thái con xe về Hết Hàng (Out_of_stock) luôn.

            return (true, "Chúc mừng! Đặt cọc thành công, xe đã được giữ riêng cho bạn.");
        }
    }
}
