using CoreEntities.Models;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;

namespace SqlServer.Repositories
{
    public class CarPricingVersionRepository : ICarPricingVersionRepository
    {
        private readonly OtoContext _context;

        public CarPricingVersionRepository(OtoContext context)
        {
            _context = context;
        }

        public async Task<List<CarPricingVersion>> GetActiveAsync()
        {
            return await _context.CarPricingVersions
                .Include(x => x.Car)
                .Where(x => x.IsActive && x.Car.IsDeleted == false)
                .OrderBy(x => x.Car.Name)
                .ThenBy(x => x.SortOrder)
                .ThenBy(x => x.PricingVersionId)
                .ToListAsync();
        }

        public async Task<List<CarPricingVersion>> GetAllAsync(int? carId = null, bool? isActive = null)
        {
            var query = _context.CarPricingVersions
                .Include(x => x.Car)
                .AsQueryable();

            if (carId.HasValue)
            {
                query = query.Where(x => x.CarId == carId.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            return await query
                .OrderBy(x => x.Car.Name)
                .ThenBy(x => x.SortOrder)
                .ThenBy(x => x.PricingVersionId)
                .ToListAsync();
        }

        public async Task<CarPricingVersion?> GetByIdAsync(int id)
        {
            return await _context.CarPricingVersions
                .Include(x => x.Car)
                .FirstOrDefaultAsync(x => x.PricingVersionId == id);
        }

        public async Task<bool> CarExistsAsync(int carId)
        {
            return await _context.Cars.AnyAsync(x => x.CarId == carId && x.IsDeleted == false);
        }

        public async Task AddAsync(CarPricingVersion entity)
        {
            await _context.CarPricingVersions.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CarPricingVersion entity)
        {
            _context.CarPricingVersions.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(CarPricingVersion entity)
        {
            _context.CarPricingVersions.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
