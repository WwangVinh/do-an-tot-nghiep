using CarSales.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CarSales.API.Repositories     //Bảng Hãng xe (Brands) phần thực hiện
{
    public class BrandRepository : IBrandRepository
    {
        private readonly CarSalesDbContext _context;

        public BrandRepository(CarSalesDbContext context)
        {
            _context = context;
        }

        public async Task<List<Brand>> GetAllAsync()
        {
            return await _context.Brands.ToListAsync();
        }

        public async Task<Brand?> GetByIdAsync(int id)
        {
            return await _context.Brands.FindAsync(id);
        }

        public async Task AddAsync(Brand brand)
        {
            _context.Brands.Add(brand);
            await SaveAsync();
        }

        public async Task UpdateAsync(Brand brand)
        {
            _context.Entry(brand).State = EntityState.Modified;
            await SaveAsync();
        }

        public async Task DeleteAsync(Brand brand)
        {
            _context.Brands.Remove(brand);
            await SaveAsync();
        }

        public bool Exists(int id)
        {
            return _context.Brands.Any(e => e.BrandId == id);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
