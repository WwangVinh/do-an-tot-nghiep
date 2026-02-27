using System.Collections.Generic;
using System.Threading.Tasks;
using OtoBackend.Models;

namespace OtoBackend.Interfaces
{
    public interface ICarRepository
    {
        // Trả về cả danh sách xe và tổng số lượng xe để FE làm nút bấm trang 1, 2, 3...
        Task<(IEnumerable<Car> Cars, int TotalCount)> GetCustomerCarsAsync(string? search, string? brand, string? color, decimal? minPrice, decimal? maxPrice, int page, int pageSize);
        // Dành riêng cho Admin: Lọc được cả xe trong thùng rác và mọi Status
        Task<(IEnumerable<Car> Cars, int TotalCount)> GetAdminCarsAsync(string? search, CarStatus? status, bool? isDeleted, int page, int pageSize);
        //Task<IEnumerable<Car>> GetAllCarsAsync();
        Task<Car> GetCarByIdAsync(int id);
        // Hàm kiểm tra trùng tên (có loại trừ ID của chính nó khi đang sửa)
        Task<bool> CheckNameExistAsync(string name, int excludeId = 0);
        Task AddCarAsync(Car car);
        Task UpdateCarAsync(Car car);
        Task DeleteCarAsync(Car car);
        Task DeleteCarAsync(int id);
        bool CarExists(int id);
    }
}