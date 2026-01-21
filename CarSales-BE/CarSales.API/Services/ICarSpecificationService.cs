using CarSales.API.Models;

namespace CarSales.API.Services
{
    public interface ICarSpecificationService
    {
        Task<List<CarSpecification>> GetAllAsync();
        Task<CarSpecification?> GetByIdAsync(int id);
        Task<CarSpecification> CreateAsync(CarSpecification carSpecification);
        Task<bool> UpdateAsync(int id, CarSpecification carSpecification);
        Task<bool> DeleteAsync(int id);
    }
}
