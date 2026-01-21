using CarSales.API.Models;

namespace CarSales.API.Repositories
{
    public interface ICarImageRepository
    {
        Task<List<CarImage>> GetAllAsync();
        Task<CarImage?> GetByIdAsync(int id);
        Task AddAsync(CarImage carImage);
        Task UpdateAsync(CarImage carImage);
        Task DeleteAsync(CarImage carImage);
        bool Exists(int id);
        Task SaveAsync();
    }
}
