using CarSales.API.Models;

namespace CarSales.API.Services
{
    public interface ICarModelService
    {
        Task<List<CarModel>> GetAllAsync();
        Task<CarModel?> GetByIdAsync(int id);
        Task<CarModel> CreateAsync(CarModel carModel);
        Task<bool> UpdateAsync(int id, CarModel carModel);
        Task<bool> DeleteAsync(int id);
    }
}
