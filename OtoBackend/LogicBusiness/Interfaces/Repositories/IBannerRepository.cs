using CoreEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface IBannerRepository
    {
        // Cho Public
        Task<IEnumerable<Banner>> GetActiveBannersByPositionAsync(int position);

        // Cho Admin
        Task<IEnumerable<Banner>> GetAllBannersAsync();
        Task<Banner?> GetBannerByIdAsync(int id);
        Task<Banner> AddBannerAsync(Banner banner);
        Task UpdateBannerAsync(Banner banner);
        Task DeleteBannerAsync(Banner banner);
    }
}
