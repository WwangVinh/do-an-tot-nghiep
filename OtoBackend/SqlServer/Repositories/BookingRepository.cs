using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }

        public async Task<Booking?> GetByIdAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Showroom)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        }

        public async Task<bool> IsTimeSlotBookedAsync(int carId, int showroomId, DateOnly bookingDate, string bookingTime)
        {
            if (!TimeSpan.TryParse(bookingTime, out TimeSpan parsedNewTime))
                return false;

            var buffer = TimeSpan.FromHours(2);
            var dailyBookings = await _context.Bookings
                .Where(b => b.CarId == carId &&
                            b.ShowroomId == showroomId &&
                            b.BookingDate == bookingDate &&
                            b.Status != BookingStatus.Cancelled &&
                            b.Status != BookingStatus.Completed &&
                            b.Status != BookingStatus.NoShow)
                .ToListAsync();

            foreach (var b in dailyBookings)
            {
                if (TimeSpan.TryParse(b.BookingTime, out TimeSpan existingTime))
                {
                    if (existingTime > parsedNewTime.Subtract(buffer) && existingTime < parsedNewTime.Add(buffer))
                        return true;
                }
            }

            return false;
        }

        public async Task<bool> HasBookedCarAsync(string phone, int carId)
        {
            return await _context.Bookings.AnyAsync(b =>
                b.Phone == phone &&
                b.CarId == carId &&
                b.Status == BookingStatus.Completed);
        }

        public async Task<(IEnumerable<Booking> Bookings, int TotalCount)> GetAdminBookingsAsync(
            int page, int pageSize, string? searchName, string? status,
            DateTime? fromDate, DateTime? toDate, int? targetShowroomId)
        {
            var query = _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Showroom)
                .AsQueryable();

            if (targetShowroomId.HasValue)
                query = query.Where(b => b.ShowroomId == targetShowroomId.Value);

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                var lower = searchName.ToLower();
                query = query.Where(b => b.CustomerName.ToLower().Contains(lower) || b.Phone.Contains(lower));
            }

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(b => b.Status == status);

            if (fromDate.HasValue)
                query = query.Where(b => b.BookingDate >= DateOnly.FromDateTime(fromDate.Value));

            if (toDate.HasValue)
                query = query.Where(b => b.BookingDate <= DateOnly.FromDateTime(toDate.Value));

            int totalCount = await query.CountAsync();

            var bookings = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (bookings, totalCount);
        }

        public async Task<IEnumerable<Booking>> GetByPhoneAsync(string phone)
        {
            return await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Showroom)
                .Where(b => b.Phone == phone)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByUserIdAsync(int userId)
        {
            return await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Showroom)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByStatusAsync(string status, int? showroomId = null)
        {
            var query = _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Showroom)
                .Where(b => b.Status == status);

            if (showroomId.HasValue)
                query = query.Where(b => b.ShowroomId == showroomId.Value);

            return await query
                .OrderBy(b => b.BookingDate)
                .ThenBy(b => b.BookingTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetPendingTechCheckAsync(int? showroomId)
        {
            var query = _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Showroom)
                .Where(b => b.Status == BookingStatus.PendingTechCheck);

            if (showroomId.HasValue)
                query = query.Where(b => b.ShowroomId == showroomId.Value);

            return await query
                .OrderBy(b => b.BookingDate)
                .ThenBy(b => b.BookingTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsTomorrowAsync()
        {
            var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

            return await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Showroom)
                .Where(b =>
                    b.BookingDate == tomorrow &&
                    (b.Status == BookingStatus.TechApproved ||
                     b.Status == BookingStatus.Confirmed))
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetOverdueBookingsAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            return await _context.Bookings
                .Where(b =>
                    b.BookingDate < today &&
                    (b.Status == BookingStatus.TechApproved ||
                     b.Status == BookingStatus.Confirmed))
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> CountByStatusAsync(int? showroomId = null)
        {
            var query = _context.Bookings.AsQueryable();

            if (showroomId.HasValue)
                query = query.Where(b => b.ShowroomId == showroomId.Value);

            return await query
                .GroupBy(b => b.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
        }
    }
}