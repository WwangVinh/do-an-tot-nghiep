using LogicBusiness.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Customer
{
    public interface IBookingService
    {
        Task<(bool Success, string Message)> CreateBookingAsync(BookingCreateDto dto);

        Task<IEnumerable<object>> GetBookingsByPhoneAsync(string phone);

        Task<object?> GetBookingDetailByPhoneAsync(int bookingId, string phone);

        Task<(bool Success, string Message)> CancelBookingByPhoneAsync(int bookingId, string phone, string? reason);
    }
}