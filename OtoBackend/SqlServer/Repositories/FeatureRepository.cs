using CoreEntities.Models;
using CoreEntities.Models;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Repositories
{
    public class FeatureRepository : IFeatureRepository
    {
        private readonly OtoContext _context;

        public FeatureRepository(OtoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Feature>> GetAllAsync(string? search = null)
        {
            var query = _context.Features.AsQueryable();

            // Nếu có gõ chữ tìm kiếm thì lọc từ khóa (Bỏ qua hoa thường)
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(f => f.FeatureName.Contains(search));
            }

            // Luôn xếp A-Z cho đẹp đội hình
            return await query.OrderBy(f => f.FeatureName).ToListAsync();
        }

        public async Task<Feature> GetByIdAsync(int id)
        {
            return await _context.Features.FindAsync(id);
        }

        public async Task AddAsync(Feature feature)
        {
            await _context.Features.AddAsync(feature);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Feature feature)
        {
            _context.Features.Update(feature);
            await _context.SaveChangesAsync();
        }

        public async Task<List<string>> GetCarNamesUsingFeatureAsync(int featureId)
        {
            // Tìm trong bảng nối CarFeature, lôi tên con xe đang chứa FeatureId này ra
            return await _context.CarFeatures
                                 .Where(cf => cf.FeatureId == featureId)
                                 .Select(cf => cf.Car.Name)
                                 .ToListAsync();
        }

        public async Task<bool> CheckExistsAsync(string featureName, int? excludeId = null)
        {
            // Tìm xem có cái tên nào trùng không (không phân biệt chữ hoa chữ thường)
            var query = _context.Features.Where(f => f.FeatureName.ToLower() == featureName.ToLower());

            // Nếu đang Edit (Cập nhật), thì bỏ qua chính cái ID đang edit
            if (excludeId.HasValue)
            {
                query = query.Where(f => f.FeatureId != excludeId.Value);
            }

            return await query.AnyAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var feature = await _context.Features.FindAsync(id);
            if (feature != null)
            {
                _context.Features.Remove(feature);
                await _context.SaveChangesAsync();
            }
        }
    }
}
