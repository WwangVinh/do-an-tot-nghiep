using CoreEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface IBookingRepository
    {
        
        Task AddAsync(Booking booking);
        Task<bool> IsTimeSlotBookedAsync(int carId, int showroomId, DateOnly bookingDate, string bookingTime);

        // Lấy chi tiết 1 lịch hẹn (kèm thông tin Xe và Showroom)
        Task<Booking?> GetByIdAsync(int bookingId);

        // Cập nhật thông tin lịch hẹn (Đổi trạng thái, ghi chú...)
        Task UpdateAsync(Booking booking);

        // Lấy danh sách lịch hẹn cho trang Quản trị (Có phân trang, lọc, search)
        Task<(IEnumerable<Booking> Bookings, int TotalCount)> GetAdminBookingsAsync(
            int page, int pageSize, string? searchName, string? status,
            DateTime? fromDate, DateTime? toDate, int? targetShowroomId);

        Task<IEnumerable<Booking>> GetByUserIdAsync(int userId);
    }
}
