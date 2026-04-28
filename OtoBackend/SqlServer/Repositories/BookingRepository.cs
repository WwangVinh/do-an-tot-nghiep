using CoreEntities.Models;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlServer.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly OtoContext _context; 

        public BookingRepository(OtoContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
        }

        // KIỂM TRA LỊCH HẸN CÓ BỊ ĐỤNG GIỜ KHÔNG (Dùng cho tạo lịch hẹn mới)
        public async Task<bool> IsTimeSlotBookedAsync(int carId, int showroomId, DateOnly bookingDate, string bookingTime)
        {
            if (!TimeSpan.TryParse(bookingTime, out TimeSpan parsedNewTime))
                return false;

            var buffer = TimeSpan.FromHours(2);
            var startTime = parsedNewTime.Subtract(buffer);
            var endTime = parsedNewTime.Add(buffer);

            // Lấy TẤT CẢ lịch hẹn của chiếc xe đó TRONG NGÀY HÔM ĐÓ lên
            // Dùng DateOnly so sánh với DateOnly là không bị báo lỗi nữa!
            var dailyBookings = await _context.Bookings
                .Where(b => b.CarId == carId &&
                            b.ShowroomId == showroomId &&
                            b.BookingDate == bookingDate &&
                            b.Status != "Cancelled" &&
                            b.Status != "Completed")
                .ToListAsync();

            // So sánh thủ công từng cái lịch xem có bị đụng múi giờ không
            foreach (var b in dailyBookings)
            {
                if (TimeSpan.TryParse(b.BookingTime, out TimeSpan existingTime))
                {
                    if (existingTime > startTime && existingTime < endTime)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // LẤY CHI TIẾT 1 LỊCH HẸN
        public async Task<Booking?> GetByIdAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Showroom)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        }

        // CẬP NHẬT LỊCH HẸN
        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }

        // LẤY DANH SÁCH CHO QUẢN LÝ
        public async Task<(IEnumerable<Booking> Bookings, int TotalCount)> GetAdminBookingsAsync(
            int page, int pageSize, string? searchName, string? status,
            DateTime? fromDate, DateTime? toDate, int? targetShowroomId)
        {
            var query = _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Showroom)
                .AsQueryable();

            //Lọc theo Chi nhánh (Manager/Staff)
            if (targetShowroomId.HasValue)
            {
                query = query.Where(b => b.ShowroomId == targetShowroomId.Value);
            }

            //Tìm kiếm theo Tên khách hàng hoặc Số điện thoại
            if (!string.IsNullOrWhiteSpace(searchName))
            {
                var lowerSearch = searchName.ToLower();
                query = query.Where(b => b.CustomerName.ToLower().Contains(lowerSearch) ||
                                         b.Phone.Contains(lowerSearch));
            }

            //Lọc theo Trạng thái (Vd: Chỉ xem các đơn "Scheduled")
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(b => b.Status == status);
            }

            //Lọc theo khoảng thời gian hẹn
            // Do DB lưu kiểu DateOnly, mà API nhận DateTime nên phải chuyển đổi nhẹ ở đây
            if (fromDate.HasValue)
            {
                var fromDateOnly = DateOnly.FromDateTime(fromDate.Value);
                query = query.Where(b => b.BookingDate >= fromDateOnly);
            }

            if (toDate.HasValue)
            {
                var toDateOnly = DateOnly.FromDateTime(toDate.Value);
                query = query.Where(b => b.BookingDate <= toDateOnly);
            }

            // Đếm tổng số lượng lịch hẹn rớt vào rổ (để FE làm phân trang)
            int totalCount = await query.CountAsync();

            // Sắp xếp: Ưu tiên hiển thị lịch hẹn mới tạo lên trên cùng
            query = query.OrderByDescending(b => b.CreatedAt);

            var bookings = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (bookings, totalCount);
        }


        //LẤY LỊCH RIÊNG (Dùng cho trang My Bookings)
        public async Task<IEnumerable<Booking>> GetByUserIdAsync(int userId)
        {
            return await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Showroom)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt) // Mới nhất lên đầu
                .ToListAsync();
        }

        public async Task<bool> HasBookedCarAsync(string phone, int carId)
        {
            // Kiểm tra xem khách đã từng đặt lịch và hoàn thành với xe này chưa
            return await _context.Bookings.AnyAsync(b => b.Phone == phone
                && b.CarId == carId
                && b.Status == "Completed");
        }
    }
}
