using CarSales.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CarSales.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CarSalesDbContext _context;

        public UserRepository(CarSalesDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await SaveAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await SaveAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await SaveAsync();
        }

        public bool Exists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
