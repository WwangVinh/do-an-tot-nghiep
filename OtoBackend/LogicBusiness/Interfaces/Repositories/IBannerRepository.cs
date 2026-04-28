using CoreEntities.Models;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface IBannerRepository
    {
        Task<List<Banner>> GetAllAsync(bool? isActive = null);
        Task<Banner?> GetByIdAsync(int id);
        Task AddAsync(Banner banner);
        Task UpdateAsync(Banner banner);
        Task DeleteAsync(Banner banner);
    }
}

