using CarSales.API.Models;

namespace CarSales.API.Services
{
    public interface ISpecificationService
    {
        Task<List<Specification>> GetAllAsync();
        Task<Specification?> GetByIdAsync(int id);
        Task<Specification> CreateAsync(Specification specification);
        Task<bool> UpdateAsync(int id, Specification specification);
        Task<bool> DeleteAsync(int id);
    }
}
