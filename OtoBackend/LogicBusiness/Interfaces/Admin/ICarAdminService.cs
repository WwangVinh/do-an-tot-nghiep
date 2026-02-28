using CoreEntities.Models;
using Microsoft.AspNetCore.Http;

namespace LogicBusiness.Interfaces.Admin
{
    public interface ICarAdminService
    {
        Task<object> GetCarsAsync(string? search, CarStatus? status, bool? isDeleted, int page, int pageSize);
        Task<object?> GetCarDetailAsync(int id);
        Task<(bool Success, string Message, Car? Data)> CreateCarAsync(Car car, IFormFile? imageFile);
        Task<(bool Success, string Message, Car? Data)> UpdateCarAsync(int id, Car car);
        Task<(bool Success, string Message, CarImage? Data)> UploadGalleryImageAsync(int carId, IFormFile file, string imageType);
        Task<(bool Success, string Message, object? Data)> Upload360ImagesAsync(int carId, List<IFormFile> files);
        Task<bool> DeleteCarImageAsync(int imageId);
        Task<bool> SoftDeleteCarAsync(int id, int deletedByUserId);
        Task<bool> RestoreCarAsync(int id);
        Task<bool> HardDeleteCarAsync(int id);
    }
}