using CarSales.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CarSales.API.Repositories
{
    public class TestDriveBookingRepository : ITestDriveBookingRepository
    {
        private readonly CarSalesDbContext _context;

        public TestDriveBookingRepository(CarSalesDbContext context)
        {
            _context = context;
        }

        public async Task<List<TestDriveBooking>> GetAllAsync()
        {
            return await _context.TestDriveBookings.ToListAsync();
        }

        public async Task<TestDriveBooking?> GetByIdAsync(int id)
        {
            return await _context.TestDriveBookings.FindAsync(id);
        }

        public async Task AddAsync(TestDriveBooking booking)
        {
            _context.TestDriveBookings.Add(booking);
            await SaveAsync();
        }

        public async Task UpdateAsync(TestDriveBooking booking)
        {
            _context.Entry(booking).State = EntityState.Modified;
            await SaveAsync();
        }

        public async Task DeleteAsync(TestDriveBooking booking)
        {
            _context.TestDriveBookings.Remove(booking);
            await SaveAsync();
        }

        public bool Exists(int id)
        {
            return _context.TestDriveBookings.Any(e => e.BookingId == id);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
