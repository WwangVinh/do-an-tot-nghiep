using CoreEntities.Models;

namespace LogicBusiness.Interfaces.Customer
{
    public interface ICarService
    {
        Task<object> GetCarsAsync(string? search, string? brand, string? color, decimal? minPrice, decimal? maxPrice, CarStatus? status, string? transmission, string? bodyStyle, int page, int pageSize);
        Task<object?> GetCarDetailAsync(int id);
    }
}