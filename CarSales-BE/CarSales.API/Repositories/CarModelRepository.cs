using CarSales.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CarSales.API.Repositories
{
    public class CarModelRepository : ICarModelRepository
    {
        private readonly CarSalesDbContext _context;

        public CarModelRepository(CarSalesDbContext context)
        {
            _context = context;
        }

        public async Task<List<CarModel>> GetAllAsync()
        {
            return await _context.CarModels.ToListAsync();
        }

        public async Task<CarModel?> GetByIdAsync(int id)
        {
            return await _context.CarModels.FindAsync(id);
        }

        public async Task AddAsync(CarModel carModel)
        {
            _context.CarModels.Add(carModel);
            await SaveAsync();
        }

        public async Task UpdateAsync(CarModel carModel)
        {
            _context.Entry(carModel).State = EntityState.Modified;
            await SaveAsync();
        }

        public async Task DeleteAsync(CarModel carModel)
        {
            _context.CarModels.Remove(carModel);
            await SaveAsync();
        }

        public bool Exists(int id)
        {
            return _context.CarModels.Any(e => e.ModelId == id);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
