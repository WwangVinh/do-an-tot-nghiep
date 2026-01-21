using CarSales.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CarSales.API.Repositories
{
    public class CarRepository : ICarRepository
    {
        private readonly CarSalesDbContext _context;

        public CarRepository(CarSalesDbContext context)
        {
            _context = context;
        }

        public async Task<List<Car>> GetAllAsync()
        {
            return await _context.Cars.ToListAsync();
        }

        public async Task<Car?> GetByIdAsync(int id)
        {
            return await _context.Cars.FindAsync(id);
        }

        public async Task AddAsync(Car car)
        {
            _context.Cars.Add(car);
            await SaveAsync();
        }

        public async Task UpdateAsync(Car car)
        {
            _context.Entry(car).State = EntityState.Modified;
            await SaveAsync();
        }

        public async Task DeleteAsync(Car car)
        {
            _context.Cars.Remove(car);
            await SaveAsync();
        }

        public bool Exists(int id)
        {
            return _context.Cars.Any(e => e.CarId == id);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
