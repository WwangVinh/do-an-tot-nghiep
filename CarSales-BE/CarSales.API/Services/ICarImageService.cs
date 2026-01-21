using CarSales.API.Models;

namespace CarSales.API.Services
{
    public interface ICarImageService
    {
        Task<List<CarImage>> GetAllAsync();
        Task<CarImage?> GetByIdAsync(int id);
        Task<CarImage> CreateAsync(CarImage carImage);
        Task<bool> UpdateAsync(int id, CarImage carImage);
        Task<bool> DeleteAsync(int id);
    }
}
