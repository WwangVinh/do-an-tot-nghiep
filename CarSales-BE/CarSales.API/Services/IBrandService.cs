using CarSales.API.Models;

namespace CarSales.API.Services     //Bảng Hãng xe (Brands) phần interface
{
    public interface IBrandService
    {
        Task<List<Brand>> GetAllAsync();
        Task<Brand?> GetByIdAsync(int id);
        Task<Brand> CreateAsync(Brand brand);
        Task<bool> UpdateAsync(int id, Brand brand);
        Task<bool> DeleteAsync(int id);
    }
}
