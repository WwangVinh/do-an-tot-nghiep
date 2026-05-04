using CoreEntities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface IBookingRepository
    {
        Task AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task<Booking?> GetByIdAsync(int bookingId);
        Task<bool> IsTimeSlotBookedAsync(int carId, int showroomId, DateOnly bookingDate, string bookingTime);
        Task<bool> HasBookedCarAsync(string phone, int carId);

        Task<(IEnumerable<Booking> Bookings, int TotalCount)> GetAdminBookingsAsync(
            int page, int pageSize, string? searchName, string? status,
            DateTime? fromDate, DateTime? toDate, int? targetShowroomId);

        Task<IEnumerable<Booking>> GetByPhoneAsync(string phone);
        Task<IEnumerable<Booking>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Booking>> GetByStatusAsync(string status, int? showroomId = null);
        Task<IEnumerable<Booking>> GetPendingTechCheckAsync(int? showroomId);
        Task<IEnumerable<Booking>> GetBookingsTomorrowAsync();
        Task<IEnumerable<Booking>> GetOverdueBookingsAsync();
        Task<Dictionary<string, int>> CountByStatusAsync(int? showroomId = null);
        Task<string?> GetCustomerNameFromBookingAsync(string phone, int carId);
    }
}