using CoreEntities.Models;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlServer.Repositories
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly OtoContext _context;
        public PromotionRepository(OtoContext context) { _context = context; }

        public async Task<IEnumerable<Promotion>> GetAllAsync()
        {
            return await _context.Promotions
                .Include(p => p.Car) // Kéo theo tên xe
                .OrderByDescending(p => p.PromotionId)
                .ToListAsync();
        }

        public async Task<Promotion?> GetByIdAsync(int id)
        {
            return await _context.Promotions.FindAsync(id);
        }

        public async Task<bool> CheckCodeExistsAsync(string code, int? excludeId = null)
        {
            var query = _context.Promotions.Where(p => p.Code == code);
            if (excludeId.HasValue) query = query.Where(p => p.PromotionId != excludeId.Value);
            return await query.AnyAsync();
        }

        public async Task AddAsync(Promotion promotion)
        {
            await _context.Promotions.AddAsync(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Promotion promotion) { _context.Promotions.Update(promotion); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(Promotion promotion) { _context.Promotions.Remove(promotion); await _context.SaveChangesAsync(); }
    }
}