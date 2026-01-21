using CarSales.API.Models;

namespace CarSales.API.Services
{
    public interface ICarService
    {
        Task<List<Car>> GetAllAsync();
        Task<Car?> GetByIdAsync(int id);
        Task<Car> CreateAsync(Car car);
        Task<bool> UpdateAsync(int id, Car car);
        Task<bool> DeleteAsync(int id);
    }
}
