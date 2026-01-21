using CarSales.API.Models;

namespace CarSales.API.Repositories
{
    public interface ISpecificationRepository
    {
        Task<List<Specification>> GetAllAsync();
        Task<Specification?> GetByIdAsync(int id);
        Task AddAsync(Specification specification);
        Task UpdateAsync(Specification specification);
        Task DeleteAsync(Specification specification);
        bool Exists(int id);
        Task SaveAsync();
    }
}
