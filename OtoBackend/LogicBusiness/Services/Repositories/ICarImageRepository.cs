using CoreEntities.Models;

namespace LogicBusiness.Services.Repositories
{
    public interface ICarImageRepository
    {
        Task<List<CarImage>> GetCarImagesAsync(int carId);
        Task<List<CarImage>> Get360ImagesAsync(int carId);
        Task<CarImage> AddCarImageAsync(CarImage carImage);
        Task<bool> UpdateImageDescriptionAsync(int imageId, string description, string title);
        Task<CarImage> GetCarImageByIdAsync(int imageId);
        Task DeleteCarImageAsync(int imageId);
        Task DeleteAllImagesByCarIdAsync(int carId);
    }
}