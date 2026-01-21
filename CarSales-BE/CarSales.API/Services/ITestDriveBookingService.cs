using CarSales.API.Models;

namespace CarSales.API.Services
{
    public interface ITestDriveBookingService
    {
        Task<List<TestDriveBooking>> GetAllAsync();
        Task<TestDriveBooking?> GetByIdAsync(int id);
        Task<TestDriveBooking> CreateAsync(TestDriveBooking booking);
        Task<bool> UpdateAsync(int id, TestDriveBooking booking);
        Task<bool> DeleteAsync(int id);
    }
}
