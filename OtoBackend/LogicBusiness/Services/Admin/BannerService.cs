using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Customer;
using LogicBusiness.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Services.Admin
{
    public class BannerService : IBannerService
    {
        private readonly IBannerRepository _repo;

        public BannerService(IBannerRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<BannerResponseDto>> GetPublicBannersAsync(int position)
        {
            var banners = await _repo.GetActiveBannersByPositionAsync(position);
            return banners.Select(MapToDto);
        }

        public async Task<IEnumerable<BannerResponseDto>> GetAllAdminBannersAsync()
        {
            var banners = await _repo.GetAllBannersAsync();
            return banners.Select(MapToDto);
        }

        public async Task<BannerResponseDto> CreateBannerAsync(BannerCreateUpdateDto dto)
        {
            var banner = new Banner
            {
                BannerName = dto.BannerName,
                ImageUrl = dto.ImageUrl,
                LinkUrl = dto.LinkUrl,
                Position = dto.Position,
                IsActive = dto.IsActive,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            var savedBanner = await _repo.AddBannerAsync(banner);
            return MapToDto(savedBanner);
        }

        // Tính năng thao tác nhanh: Bật/Tắt banner 1 chạm
        public async Task<bool> ToggleBannerStatusAsync(int id)
        {
            var banner = await _repo.GetBannerByIdAsync(id);
            if (banner == null) return false;

            banner.IsActive = !banner.IsActive; // Đảo ngược trạng thái
            await _repo.UpdateBannerAsync(banner);
            return true;
        }

        // Hàm phụ trợ để code đỡ lặp lại
        private static BannerResponseDto MapToDto(Banner b)
        {
            return new BannerResponseDto
            {
                BannerId = b.BannerId,
                BannerName = b.BannerName,
                ImageUrl = b.ImageUrl,
                LinkUrl = b.LinkUrl,
                Position = b.Position,
                IsActive = b.IsActive,
                StartDate = b.StartDate,
                EndDate = b.EndDate
            };
        }
    }
}
