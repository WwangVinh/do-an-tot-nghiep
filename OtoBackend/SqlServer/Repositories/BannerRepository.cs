using CoreEntities.Models;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlServer.Repositories
{
    public class BannerRepository : IBannerRepository
    {
        private readonly OtoContext _context;

        public BannerRepository(OtoContext context)
        {
            _context = context;
        }

        // Lọc cực kỳ nghiêm ngặt cho FE Khách hàng
        public async Task<IEnumerable<Banner>> GetActiveBannersByPositionAsync(int position)
        {
            var now = DateTime.Now;
            return await _context.Banners
                .Where(b => b.Position == position
                         && b.IsActive == true
                         // Nếu StartDate NULL thì coi như chạy từ quá khứ, nếu có thì phải <= hiện tại
                         && (!b.StartDate.HasValue || b.StartDate <= now)
                         // Nếu EndDate NULL thì coi như chạy vô thời hạn, nếu có thì phải >= hiện tại
                         && (!b.EndDate.HasValue || b.EndDate >= now))
                .OrderByDescending(b => b.BannerId)
                .ToListAsync();
        }

        // Dành cho Admin: Lấy hết không che
        public async Task<IEnumerable<Banner>> GetAllBannersAsync()
        {
            return await _context.Banners.OrderByDescending(b => b.BannerId).ToListAsync();
        }

        public async Task<Banner?> GetBannerByIdAsync(int id) => await _context.Banners.FindAsync(id);

        public async Task<Banner> AddBannerAsync(Banner banner)
        {
            await _context.Banners.AddAsync(banner);
            await _context.SaveChangesAsync();
            return banner;
        }

        public async Task UpdateBannerAsync(Banner banner)
        {
            _context.Banners.Update(banner);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBannerAsync(Banner banner)
        {
            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync();
        }
    }
}
