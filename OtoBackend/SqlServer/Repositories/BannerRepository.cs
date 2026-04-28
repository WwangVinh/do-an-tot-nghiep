using CoreEntities.Models;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;
using LogicBusiness.Utilities;
using LogicBusiness.Interfaces.Repositories;

namespace LogicBusiness.Repositories
{
    public class BannerRepository : IBannerRepository
    {
        private readonly OtoContext _context;

        public BannerRepository(OtoContext context)
        {
            _context = context;
        }

        public async Task<List<Banner>> GetAllAsync(bool? isActive = null)
        {
            var query = _context.Banners.AsNoTracking().AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(b => b.IsActive == isActive.Value);
            }

            return await query
                .OrderBy(b => b.Position)
                .ThenByDescending(b => b.BannerId)
                .ToListAsync();
        }

        public async Task<Banner?> GetByIdAsync(int id)
        {
            return await _context.Banners.FirstOrDefaultAsync(b => b.BannerId == id);
        }

        public async Task AddAsync(Banner banner)
        {
            await _context.Banners.AddAsync(banner);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Banner banner)
        {
            _context.Banners.Update(banner);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Banner banner)
        {
            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync();
        }
    }
}

