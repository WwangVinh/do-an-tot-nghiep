using CarSales.API.Models;
using CarSales.API.Repositories;

namespace CarSales.API.Services
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _repository;

        public CarService(ICarRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Car>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Car?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Car> CreateAsync(Car car)
        {
            await _repository.AddAsync(car);
            return car;
        }

        public async Task<bool> UpdateAsync(int id, Car car)
        {
            if (id != car.CarId || !_repository.Exists(id))
                return false;

            await _repository.UpdateAsync(car);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var car = await _repository.GetByIdAsync(id);
            if (car == null) return false;

            await _repository.DeleteAsync(car);
            return true;
        }
    }
}
