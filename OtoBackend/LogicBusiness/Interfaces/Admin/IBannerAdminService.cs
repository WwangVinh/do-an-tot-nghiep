using CoreEntities.Models;
using LogicBusiness.DTOs;

namespace LogicBusiness.Interfaces.Admin
{
    public interface IBannerAdminService
    {
        Task<List<Banner>> GetAllAsync(bool? isActive = null);
        Task<Banner?> GetByIdAsync(int id);
        Task<(bool Success, string Message, Banner? Data)> CreateAsync(BannerCreateDto dto, int currentUserId);
        Task<(bool Success, string Message, Banner? Data)> UpdateAsync(int id, BannerUpdateDto dto, int currentUserId);
        Task<(bool Success, string Message)> DeleteAsync(int id, int currentUserId);
    }
}

