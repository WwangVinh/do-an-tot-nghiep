using CoreEntities.Models;

namespace LogicBusiness.Interfaces.Customer
{
    public interface ICarService
    {
        Task<object> GetCarsAsync(
            string? search, string? brand, string? color,
            decimal? minPrice, decimal? maxPrice, CarStatus? status,
            string? transmission, string? bodyStyle,
            string? fuelType, string? location,
            CarCondition? condition, int? minYear, int? maxYear,
            string? sort, bool inStockOnly,
            int page, int pageSize);
        Task<object?> GetCarDetailAsync(int id);
        Task<IEnumerable<object>> GetLatestCarsAsync(int limit);
        Task<IEnumerable<object>> GetBestSellingCarsAsync(int limit);
    }
}