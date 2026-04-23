using LogicBusiness.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Customer
{
    public interface IBannerService
    {
        Task<IEnumerable<BannerResponseDto>> GetPublicBannersAsync(int position);
        Task<IEnumerable<BannerResponseDto>> GetAllAdminBannersAsync();
        Task<BannerResponseDto> CreateBannerAsync(BannerCreateUpdateDto dto);
        Task<bool> ToggleBannerStatusAsync(int id);
    }
}
