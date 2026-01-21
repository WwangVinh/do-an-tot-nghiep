using CarSales.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CarSales.API.Repositories
{
    public class SpecificationRepository : ISpecificationRepository
    {
        private readonly CarSalesDbContext _context;

        public SpecificationRepository(CarSalesDbContext context)
        {
            _context = context;
        }

        public async Task<List<Specification>> GetAllAsync()
        {
            return await _context.Specifications.ToListAsync();
        }

        public async Task<Specification?> GetByIdAsync(int id)
        {
            return await _context.Specifications.FindAsync(id);
        }

        public async Task AddAsync(Specification specification)
        {
            _context.Specifications.Add(specification);
            await SaveAsync();
        }

        public async Task UpdateAsync(Specification specification)
        {
            _context.Entry(specification).State = EntityState.Modified;
            await SaveAsync();
        }

        public async Task DeleteAsync(Specification specification)
        {
            _context.Specifications.Remove(specification);
            await SaveAsync();
        }

        public bool Exists(int id)
        {
            return _context.Specifications.Any(e => e.SpecId == id);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
