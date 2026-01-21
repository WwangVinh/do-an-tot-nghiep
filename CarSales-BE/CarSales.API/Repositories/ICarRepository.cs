using CarSales.API.Models;

namespace CarSales.API.Repositories
{
    public interface ICarRepository
    {
        Task<List<Car>> GetAllAsync();
        Task<Car?> GetByIdAsync(int id);
        Task AddAsync(Car car);
        Task UpdateAsync(Car car);
        Task DeleteAsync(Car car);
        bool Exists(int id);
        Task SaveAsync();
    }
}
