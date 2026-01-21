using CarSales.API.Models;

namespace CarSales.API.Repositories
{
    public interface ITestDriveBookingRepository
    {
        Task<List<TestDriveBooking>> GetAllAsync();
        Task<TestDriveBooking?> GetByIdAsync(int id);
        Task AddAsync(TestDriveBooking booking);
        Task UpdateAsync(TestDriveBooking booking);
        Task DeleteAsync(TestDriveBooking booking);
        bool Exists(int id);
        Task SaveAsync();
    }
}
