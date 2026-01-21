using CarSales.API.Models;

namespace CarSales.API.Repositories
{
    public interface ICarModelRepository
    {
        Task<List<CarModel>> GetAllAsync();
        Task<CarModel?> GetByIdAsync(int id);
        Task AddAsync(CarModel carModel);
        Task UpdateAsync(CarModel carModel);
        Task DeleteAsync(CarModel carModel);
        bool Exists(int id);
        Task SaveAsync();
    }
}
