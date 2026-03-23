using CoreEntities.Models;
using LogicBusiness.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface ICarRepository
    {
        // Dùng cho Customer: Lấy chi tiết xe và bốc luôn cả kho ảnh đi kèm
        Task<Car?> GetCarDetailForCustomerAsync(int id);

        // Dành cho Admin: Lấy chi tiết xe + Toàn bộ ảnh (Bất chấp trạng thái Nháp hay Đã xóa)
        Task<Car?> GetCarDetailForAdminAsync(int id);


        //        // Trả về cả danh sách xe và tổng số lượng xe để FE làm nút bấm trang 1, 2, 3...
        //        Task<(IEnumerable<Car> Cars, int TotalCount)> GetCustomerCarsAsync(
        //           string? search, string? brand, string? color, decimal? minPrice, decimal? maxPrice, CarStatus? status, string? fuelType, string? location,
        //           string? transmission, string? bodyStyle, int page, int pageSize);
        //v
        //        // Dành riêng cho Admin: Lọc được cả xe trong thùng rác và mọi Status
        //Task<(IEnumerable<Car> Cars, int TotalCount)> GetAdminCarsAsync(
        //        string? search, string? brand, string? color,
        //        decimal? minPrice, decimal? maxPrice, CarStatus? status,
        //        string? transmission, string? bodyStyle,
        //        string? fuelType, string? location,
        //        bool? isDeleted, int page, int pageSize);

        // Hàm lấy danh sách xe cho Khách (12 tham số)
        Task<(IEnumerable<Car> Cars, int TotalCount)> GetCustomerCarsAsync(
            string? search, string? brand, string? color,
            decimal? minPrice, decimal? maxPrice, CarStatus? status,
            string? transmission, string? bodyStyle,
            string? fuelType, string? location,
            int page, int pageSize);

        // Hàm lấy danh sách xe cho Admin (13 tham số - có thêm isDeleted)
        Task<(IEnumerable<Car> Cars, int TotalCount)> GetAdminCarsAsync(
            string? search, string? brand, string? color,
            decimal? minPrice, decimal? maxPrice, CarStatus? status,
            string? transmission, string? bodyStyle,
            string? fuelType, string? location,
            bool? isDeleted, int page, int pageSize, int? userShowroomId = null);


        Task<Car> GetCarByIdAsync(int id);

        Task<bool> CheckCarListingExistAsync(string name, string brand, int? year, string color, int condition, decimal? mileage, int? excludeId = null);

        Task<Car?> GetExistingNewCarAsync(string name, string brand, int year);
        Task AddCarAsync(Car car);
        Task UpdateCarAsync(Car car);
        // Trong file ICarRepository.cs
        Task<Car?> GetByIdAsync(int id);
        Task UpdateAsync(Car car);
        //Task DeleteCarAsync(Car car);
        Task<bool> DeleteCarAsync(int id);
        bool CarExists(int id);

        Task<IEnumerable<Car>> SearchMasterCarsAsync(string query);
    }
}