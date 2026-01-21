using CarSales.API.Models;

namespace CarSales.API.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        bool Exists(int id);
        Task SaveAsync();
    }
}
