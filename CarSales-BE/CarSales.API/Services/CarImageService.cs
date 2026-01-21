using CarSales.API.Models;
using CarSales.API.Repositories;

namespace CarSales.API.Services
{
    public class CarImageService : ICarImageService
    {
        private readonly ICarImageRepository _repository;

        public CarImageService(ICarImageRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CarImage>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<CarImage?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<CarImage> CreateAsync(CarImage carImage)
        {
            await _repository.AddAsync(carImage);
            return carImage;
        }

        public async Task<bool> UpdateAsync(int id, CarImage carImage)
        {
            if (id != carImage.ImageId || !_repository.Exists(id))
                return false;

            await _repository.UpdateAsync(carImage);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var carImage = await _repository.GetByIdAsync(id);
            if (carImage == null) return false;

            await _repository.DeleteAsync(carImage);
            return true;
        }
    }
}
