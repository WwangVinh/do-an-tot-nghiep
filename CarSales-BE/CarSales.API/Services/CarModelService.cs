using CarSales.API.Models;
using CarSales.API.Repositories;

namespace CarSales.API.Services
{
    public class CarModelService : ICarModelService
    {
        private readonly ICarModelRepository _repository;

        public CarModelService(ICarModelRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CarModel>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<CarModel?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<CarModel> CreateAsync(CarModel carModel)
        {
            await _repository.AddAsync(carModel);
            return carModel;
        }

        public async Task<bool> UpdateAsync(int id, CarModel carModel)
        {
            if (id != carModel.ModelId || !_repository.Exists(id))
                return false;

            await _repository.UpdateAsync(carModel);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var carModel = await _repository.GetByIdAsync(id);
            if (carModel == null) return false;

            await _repository.DeleteAsync(carModel);
            return true;
        }
    }
}
