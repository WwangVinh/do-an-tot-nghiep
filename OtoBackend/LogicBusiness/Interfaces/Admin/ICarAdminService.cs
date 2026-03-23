using CoreEntities.Models;
using LogicBusiness.DTOs; // 👉 THÊM DÒNG NÀY ĐỂ NHẬN DTO
using Microsoft.AspNetCore.Http;

namespace LogicBusiness.Interfaces.Admin
{
    public interface ICarAdminService
    {
        Task<object> GetCarsAsync(string? search, string? brand, string? color, decimal? minPrice, decimal? maxPrice, CarStatus? status, string? transmission, string? bodyStyle, string? fuelType, string? location, bool? isDeleted, int page, int pageSize, int? userShowroomId = null);

        // =============================
        Task<object?> GetCarDetailAsync(int id, string userRole, int? userShowroomId);
        Task<(bool Success, string Message, Car? Data)> CreateCarAsync(CarCreateDto dto, string userRole, int? userShowroomId);

        Task<(bool Success, string Message, Car? Car)> UpdateCarAsync(int id, CarUpdateDto dto, string userRole, int? userShowroomId);

        Task<(bool Success, string Message)> ApproveCarAsync(int carId, string userRole, int? userShowroomId);
        // =============================

        Task<(bool Success, string Message, object? Data)> UploadGalleryImagesAsync(int carId, List<IFormFile> files, List<string>? titles, List<string>? descriptions, string imageType);

        Task<bool> UpdateImageDetailsAsync(int imageId, string? title, string? description);

        Task<(bool Success, string Message, int? NewCarId)> CloneCarAsync(int id, string userRole, int? userShowroomId);

        Task<(bool Success, string Message, object? Data)> Upload360ImagesAsync(int carId, List<IFormFile> files);

        Task<bool> DeleteCarImageAsync(int imageId);

        Task<bool> SoftDeleteCarAsync(int id, int deletedByUserId);

        Task<bool> RestoreCarAsync(int id);

        Task<bool> HardDeleteCarAsync(int id, string userRole);

        Task<(bool Success, string Message)> RejectCarAsync(int carId, string reason);

        Task<(bool Success, string Message)> ChangeCarStatusAsync(int carId, CarStatus newStatus);
    }
}