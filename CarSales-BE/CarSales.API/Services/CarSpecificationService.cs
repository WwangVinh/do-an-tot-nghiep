using CarSales.API.Models;
using CarSales.API.Repositories;

namespace CarSales.API.Services
{
    public class CarSpecificationService : ICarSpecificationService
    {
        private readonly ICarSpecificationRepository _repository;

        public CarSpecificationService(ICarSpecificationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CarSpecification>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<CarSpecification?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<CarSpecification> CreateAsync(CarSpecification carSpecification)
        {
            await _repository.AddAsync(carSpecification);
            return carSpecification;
        }

        public async Task<bool> UpdateAsync(int id, CarSpecification carSpecification)
        {
            if (id != carSpecification.Id || !_repository.Exists(id))
                return false;

            await _repository.UpdateAsync(carSpecification);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var carSpecification = await _repository.GetByIdAsync(id);
            if (carSpecification == null) return false;

            await _repository.DeleteAsync(carSpecification);
            return true;
        }
    }
}
