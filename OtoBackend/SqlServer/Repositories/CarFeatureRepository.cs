using CoreEntities.Models;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;

namespace SqlServer.Repositories
{
    public class CarFeatureRepository : ICarFeatureRepository
    {
        private readonly OtoContext _context;

        public CarFeatureRepository(OtoContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<CarFeature> carFeatures)
        {
            await _context.CarFeatures.AddRangeAsync(carFeatures);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByCarIdAsync(int carId)
        {
            // Tìm tất cả các "sợi dây" đang nối vào con xe này
            var itemsToDelete = await _context.CarFeatures
                                              .Where(x => x.CarId == carId)
                                              .ToListAsync();

            if (itemsToDelete.Any())
            {
                // Cắt đứt hết
                _context.CarFeatures.RemoveRange(itemsToDelete);
                await _context.SaveChangesAsync();
            }
        }
    }
}