using CarSales.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CarSales.API.Repositories
{
    public class CarSpecificationRepository : ICarSpecificationRepository
    {
        private readonly CarSalesDbContext _context;

        public CarSpecificationRepository(CarSalesDbContext context)
        {
            _context = context;
        }

        public async Task<List<CarSpecification>> GetAllAsync()
        {
            return await _context.CarSpecifications.ToListAsync();
        }

        public async Task<CarSpecification?> GetByIdAsync(int id)
        {
            return await _context.CarSpecifications.FindAsync(id);
        }

        public async Task AddAsync(CarSpecification carSpecification)
        {
            _context.CarSpecifications.Add(carSpecification);
            await SaveAsync();
        }

        public async Task UpdateAsync(CarSpecification carSpecification)
        {
            _context.Entry(carSpecification).State = EntityState.Modified;
            await SaveAsync();
        }

        public async Task DeleteAsync(CarSpecification carSpecification)
        {
            _context.CarSpecifications.Remove(carSpecification);
            await SaveAsync();
        }

        public bool Exists(int id)
        {
            return _context.CarSpecifications.Any(e => e.Id == id);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
