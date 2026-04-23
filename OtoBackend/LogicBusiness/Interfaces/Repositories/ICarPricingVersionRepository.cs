using CoreEntities.Models;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface ICarPricingVersionRepository
    {
        Task<List<CarPricingVersion>> GetActiveAsync();
        Task<List<CarPricingVersion>> GetAllAsync(int? carId = null, bool? isActive = null);
        Task<CarPricingVersion?> GetByIdAsync(int id);
        Task<bool> CarExistsAsync(int carId);
        Task AddAsync(CarPricingVersion entity);
        Task UpdateAsync(CarPricingVersion entity);
        Task DeleteAsync(CarPricingVersion entity);
    }
}
