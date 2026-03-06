using CoreEntities.Models;
using LogicBusiness.DTOs; // 👉 THÊM DÒNG NÀY ĐỂ NHẬN DTO
using Microsoft.AspNetCore.Http;

namespace LogicBusiness.Interfaces.Admin
{
    public interface ICarAdminService
    {
        Task<object> GetCarsAsync(string? search, CarStatus? status, bool? isDeleted, int page, int pageSize);

        Task<object?> GetCarDetailAsync(int id);

        // 👉 SỬA LẠI DÒNG NÀY: Gom Car và IFormFile thành 1 cục DTO duy nhất
        Task<(bool Success, string Message, Car? Data)> CreateCarAsync(CarCreateDto dto);

        //Task<(bool Success, string Message, Car? Data)> UpdateCarAsync(int id, Car car);
        Task<(bool Success, string Message, Car? Car)> UpdateCarAsync(int id, CarUpdateDto dto);
        Task<(bool Success, string Message, object? Data)> UploadGalleryImagesAsync(int carId, List<IFormFile> files, List<string>? titles, List<string>? descriptions, string imageType);

        Task<bool> UpdateImageDetailsAsync(int imageId, string? title, string? description);

        Task<(bool Success, string Message, object? Data)> Upload360ImagesAsync(int carId, List<IFormFile> files);

        Task<bool> DeleteCarImageAsync(int imageId);

        Task<bool> SoftDeleteCarAsync(int id, int deletedByUserId);

        Task<bool> RestoreCarAsync(int id);

        Task<bool> HardDeleteCarAsync(int id);
    }
}