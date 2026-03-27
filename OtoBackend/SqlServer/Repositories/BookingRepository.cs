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
        private readonly OtoContext _context; // Dùng đúng OtoContext của ní

        public BookingRepository(OtoContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Booking booking)
        {
            // Thêm đơn hàng mới vào bảng Bookings
            await _context.Bookings.AddAsync(booking);

            // Chốt hạ, lưu xuống SQL Server
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsTimeSlotBookedAsync(int carId, int showroomId, DateOnly bookingDate, string bookingTime)
        {
            // 1. Chuyển cái chuỗi "09:00" của khách thành kiểu Thời gian để tính toán
            if (!TimeSpan.TryParse(bookingTime, out TimeSpan parsedNewTime))
                return false; // Nếu FE gửi lỗi định dạng thì cho qua luôn

            // Thiết lập Buffer time: 2 tiếng 1 ca
            var buffer = TimeSpan.FromHours(2);
            var startTime = parsedNewTime.Subtract(buffer);
            var endTime = parsedNewTime.Add(buffer);

            // 2. Lấy TẤT CẢ lịch hẹn của chiếc xe đó TRONG NGÀY HÔM ĐÓ lên
            // Dùng DateOnly so sánh với DateOnly là không bị báo lỗi nữa!
            var dailyBookings = await _context.Bookings
                .Where(b => b.CarId == carId &&
                            b.ShowroomId == showroomId &&
                            b.BookingDate == bookingDate &&
                            b.Status != "Cancelled" &&
                            b.Status != "Completed")
                .ToListAsync();

            // 3. So sánh thủ công từng cái lịch xem có bị đụng múi giờ không
            foreach (var b in dailyBookings)
            {
                if (TimeSpan.TryParse(b.BookingTime, out TimeSpan existingTime))
                {
                    if (existingTime > startTime && existingTime < endTime)
                    {
                        return true; // Dính chưởng! Đã có người đặt sát giờ này.
                    }
                }
            }

            return false; // Ngon lành, giờ này trống!
        }
        // 3. LẤY CHI TIẾT 1 LỊCH HẸN
        public async Task<Booking?> GetByIdAsync(int bookingId)
        {
            // Nhớ Include thêm Car và Showroom để tí nữa lấy tên hiển thị ra màn hình
            return await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Showroom)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        }

        // 4. CẬP NHẬT LỊCH HẸN
        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }

        // 5. LẤY DANH SÁCH CHO QUẢN LÝ (Có màng lọc An ninh)
        public async Task<(IEnumerable<Booking> Bookings, int TotalCount)> GetAdminBookingsAsync(
            int page, int pageSize, string? searchName, string? status,
            DateTime? fromDate, DateTime? toDate, int? targetShowroomId)
        {
            var query = _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Showroom)
                .AsQueryable();

            // --- A. BỘ LỌC DỮ LIỆU ---

            // 1. Lọc theo Chi nhánh (Quyền lực của Manager/Staff)
            if (targetShowroomId.HasValue)
            {
                query = query.Where(b => b.ShowroomId == targetShowroomId.Value);
            }

            // 2. Tìm kiếm theo Tên khách hàng hoặc Số điện thoại
            if (!string.IsNullOrWhiteSpace(searchName))
            {
                var lowerSearch = searchName.ToLower();
                query = query.Where(b => b.CustomerName.ToLower().Contains(lowerSearch) ||
                                         b.Phone.Contains(lowerSearch));
            }

            // 3. Lọc theo Trạng thái (Vd: Chỉ xem các đơn "Scheduled")
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(b => b.Status == status);
            }

            // 4. Lọc theo khoảng thời gian hẹn
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

            // --- B. CHỐT HẠ & PHÂN TRANG ---

            // Đếm tổng số lượng lịch hẹn rớt vào rổ (để FE làm cục phân trang 1, 2, 3...)
            int totalCount = await query.CountAsync();

            // Sắp xếp: Ưu tiên hiển thị lịch hẹn mới tạo lên trên cùng
            query = query.OrderByDescending(b => b.CreatedAt);

            // Cắt lát dữ liệu theo trang
            var bookings = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (bookings, totalCount);
        }


        // 1. Lấy lịch của riêng 1 khách (Dùng cho trang My Bookings)
        public async Task<IEnumerable<Booking>> GetByUserIdAsync(int userId)
        {
            return await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Showroom)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt) // Mới nhất lên đầu
                .ToListAsync();
        }
    }
}
