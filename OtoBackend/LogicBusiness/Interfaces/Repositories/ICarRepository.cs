using CoreEntities.Models;
using LogicBusiness.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface ICarRepository
    {

        Task<Car?> GetCarDetailForCustomerAsync(int id);
        Task<Car?> GetCarDetailForAdminAsync(int id);
        Task<(IEnumerable<Car> Cars, int TotalCount)> GetCustomerCarsAsync(
            string? search, string? brand, string? color,
            decimal? minPrice, decimal? maxPrice, CarStatus? status,
            string? transmission, string? bodyStyle,
            string? fuelType, string? location,
            CarCondition? condition, int? minYear, int? maxYear,
            string? sort, bool inStockOnly,
            int page, int pageSize);

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
        Task<Car?> GetByIdAsync(int id);
        Task UpdateAsync(Car car);
        Task<bool> DeleteCarAsync(int id);
        bool CarExists(int id);

        Task<IEnumerable<Car>> SearchMasterCarsAsync(string query);

        Task<IEnumerable<Car>> GetLatestCustomerCarsAsync(int limit);

        Task<IEnumerable<Car>> GetBestSellingCustomerCarsAsync(int limit);

        Task<IEnumerable<PricingCarBaseDto>> GetCarsForPricingAsync(string? brand = null);
    }
}