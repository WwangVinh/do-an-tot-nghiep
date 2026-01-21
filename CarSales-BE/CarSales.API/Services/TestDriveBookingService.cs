using CarSales.API.Models;
using CarSales.API.Repositories;

namespace CarSales.API.Services
{
    public class TestDriveBookingService : ITestDriveBookingService
    {
        private readonly ITestDriveBookingRepository _repository;

        public TestDriveBookingService(ITestDriveBookingRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TestDriveBooking>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<TestDriveBooking?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<TestDriveBooking> CreateAsync(TestDriveBooking booking)
        {
            await _repository.AddAsync(booking);
            return booking;
        }

        public async Task<bool> UpdateAsync(int id, TestDriveBooking booking)
        {
            if (id != booking.BookingId || !_repository.Exists(id))
                return false;

            await _repository.UpdateAsync(booking);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var booking = await _repository.GetByIdAsync(id);
            if (booking == null) return false;

            await _repository.DeleteAsync(booking);
            return true;
        }
    }
}
