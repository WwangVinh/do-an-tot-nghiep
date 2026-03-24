using LogicBusiness.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Customer
{
    public interface IBookingService
    {
        // Khách đặt lịch mới
        Task<(bool Success, string Message)> CreateBookingAsync(BookingCreateDto dto);

        // Khách xem lại danh sách lịch của mình
        Task<IEnumerable<object>> GetMyBookingsAsync(int userId);

        // Khách tự hủy lịch
        Task<(bool Success, string Message)> CancelBookingAsync(int bookingId, int userId);
    }
}