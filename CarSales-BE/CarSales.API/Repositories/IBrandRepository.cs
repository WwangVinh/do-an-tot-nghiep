using CarSales.API.Models;

namespace CarSales.API.Repositories     //Bảng Hãng xe (Brands) phần interface
{
    public interface IBrandRepository
    {
        Task<List<Brand>> GetAllAsync();
        Task<Brand?> GetByIdAsync(int id);
        Task AddAsync(Brand brand);
        Task UpdateAsync(Brand brand);
        Task DeleteAsync(Brand brand);
        bool Exists(int id);
        Task SaveAsync();
    }
}