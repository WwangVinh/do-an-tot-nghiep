using CoreEntities.Models;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;

namespace SqlServer.Repositories
{
    public class AccessoryRepository : IAccessoryRepository
    {
        private readonly OtoContext _context;

        public AccessoryRepository(OtoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Accessory>> GetAllAsync()
            => await _context.Accessories.OrderBy(a => a.Name).ToListAsync();

        public async Task<Accessory?> GetByIdAsync(int id)
            => await _context.Accessories.FindAsync(id);

        public async Task AddAsync(Accessory accessory)
        {
            await _context.Accessories.AddAsync(accessory);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Accessory accessory)
        {
            _context.Accessories.Update(accessory);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Accessories.FindAsync(id);
            if (entity == null) return false;
            _context.Accessories.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        // ── CarAccessory ─────────────────────────────────────────────────────────

        public async Task<IEnumerable<Accessory>> GetByCarIdAsync(int carId)
            => await _context.CarAccessories
                .Where(ca => ca.CarId == carId)
                .Include(ca => ca.Accessory)
                .Select(ca => ca.Accessory!)
                .Where(a => a.IsActive)
                .OrderBy(a => a.Name)
                .ToListAsync();

        public async Task AssignToCarAsync(int carId, List<int> accessoryIds)
        {
            var existing = await _context.CarAccessories
                .Where(ca => ca.CarId == carId)
                .Select(ca => ca.AccessoryId)
                .ToListAsync();

            var toAdd = accessoryIds
                .Where(id => !existing.Contains(id))
                .Select(id => new CarAccessory { CarId = carId, AccessoryId = id });

            await _context.CarAccessories.AddRangeAsync(toAdd);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromCarAsync(int carId, List<int> accessoryIds)
        {
            var toRemove = await _context.CarAccessories
                .Where(ca => ca.CarId == carId && accessoryIds.Contains(ca.AccessoryId))
                .ToListAsync();

            _context.CarAccessories.RemoveRange(toRemove);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsAssignedToCarAsync(int carId, int accessoryId)
            => await _context.CarAccessories
                .AnyAsync(ca => ca.CarId == carId && ca.AccessoryId == accessoryId);
    }
}