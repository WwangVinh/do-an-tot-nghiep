using CoreEntities.Models;
using Microsoft.AspNetCore.Http;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface ICarImageRepository
    {
        Task<List<CarImage>> GetCarImagesAsync(int carId);
        Task<List<CarImage>> Get360ImagesAsync(int carId);
        Task<CarImage> AddCarImageAsync(CarImage carImage);
        Task<string> UploadImageAsync(IFormFile file, string folderName);
        Task<bool> UpdateImageDescriptionAsync(int imageId, string description, string title);
        Task<CarImage> GetCarImageByIdAsync(int imageId);
        Task DeleteCarImageAsync(int imageId);
        Task<bool> DeleteAll360ImagesByCarIdAsync(int carId);
        Task DeleteAllImagesByCarIdAsync(int carId);
    }
}