using CoreEntities.Models;
using LogicBusiness.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicBusiness.Services.Repositories
{
    public interface ICarRepository
    {
        // Dùng cho Customer: Lấy chi tiết xe và bốc luôn cả kho ảnh đi kèm
        Task<Car?> GetCarDetailForCustomerAsync(int id);

        // Dành cho Admin: Lấy chi tiết xe + Toàn bộ ảnh (Bất chấp trạng thái Nháp hay Đã xóa)
        Task<Car?> GetCarDetailForAdminAsync(int id);
        // Trả về cả danh sách xe và tổng số lượng xe để FE làm nút bấm trang 1, 2, 3...
        Task<(IEnumerable<Car> Cars, int TotalCount)> GetCustomerCarsAsync(
        string? search, 
        string? brand, 
        string? color, 
        decimal? minPrice, 
        decimal? maxPrice, 
        CarStatus? status,
        int page, 
        int pageSize);

        // Dành riêng cho Admin: Lọc được cả xe trong thùng rác và mọi Status
        Task<(IEnumerable<Car> Cars, int TotalCount)> GetAdminCarsAsync(string? search, CarStatus? status, bool? isDeleted, int page, int pageSize);
        //Task<IEnumerable<Car>> GetAllCarsAsync();
        Task<Car> GetCarByIdAsync(int id);

        Task<bool> CheckCarListingExistAsync(string name, string brand, int? year, string color, int condition, decimal? mileage, int? excludeId = null);
        Task AddCarAsync(Car car);
        Task UpdateCarAsync(Car car);
        // Trong file ICarRepository.cs
        Task<Car?> GetByIdAsync(int id);
        Task UpdateAsync(Car car);
        //Task DeleteCarAsync(Car car);
        Task<bool> DeleteCarAsync(int id);
        bool CarExists(int id);
    }
}