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

        // Thêm nhiều tính năng cho một chiếc xe (Dùng khi tạo hoặc cập nhật xe)
        public async Task AddRangeAsync(IEnumerable<CarFeature> carFeatures)
        {
            await _context.CarFeatures.AddRangeAsync(carFeatures);
            await _context.SaveChangesAsync();
        }

        // Xóa tất cả tính năng của một chiếc xe (Dùng khi cập nhật xe, xóa xe)
        public async Task DeleteByCarIdAsync(int carId)
        {
            var features = await _context.CarFeatures.Where(x => x.CarId == carId).ToListAsync();
            if (features.Any())
            {
                _context.CarFeatures.RemoveRange(features);
                await _context.SaveChangesAsync();
            }
        }
    }
}