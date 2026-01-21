using CarSales.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CarSales.API.Repositories
{
    public class CarImageRepository : ICarImageRepository
    {
        private readonly CarSalesDbContext _context;

        public CarImageRepository(CarSalesDbContext context)
        {
            _context = context;
        }

        public async Task<List<CarImage>> GetAllAsync()
        {
            return await _context.CarImages.ToListAsync();
        }

        public async Task<CarImage?> GetByIdAsync(int id)
        {
            return await _context.CarImages.FindAsync(id);
        }

        public async Task AddAsync(CarImage carImage)
        {
            _context.CarImages.Add(carImage);
            await SaveAsync();
        }

        public async Task UpdateAsync(CarImage carImage)
        {
            _context.Entry(carImage).State = EntityState.Modified;
            await SaveAsync();
        }

        public async Task DeleteAsync(CarImage carImage)
        {
            _context.CarImages.Remove(carImage);
            await SaveAsync();
        }

        public bool Exists(int id)
        {
            return _context.CarImages.Any(e => e.ImageId == id);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
