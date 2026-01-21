using CarSales.API.Models;

namespace CarSales.API.Repositories
{
    public interface ICarSpecificationRepository
    {
        Task<List<CarSpecification>> GetAllAsync();
        Task<CarSpecification?> GetByIdAsync(int id);
        Task AddAsync(CarSpecification carSpecification);
        Task UpdateAsync(CarSpecification carSpecification);
        Task DeleteAsync(CarSpecification carSpecification);
        bool Exists(int id);
        Task SaveAsync();
    }
}
