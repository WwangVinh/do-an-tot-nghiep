using CarSales.API.Models;
using CarSales.API.Repositories;

namespace CarSales.API.Services
{
    public class SpecificationService : ISpecificationService
    {
        private readonly ISpecificationRepository _repository;

        public SpecificationService(ISpecificationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Specification>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Specification?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Specification> CreateAsync(Specification specification)
        {
            await _repository.AddAsync(specification);
            return specification;
        }

        public async Task<bool> UpdateAsync(int id, Specification specification)
        {
            if (id != specification.SpecId || !_repository.Exists(id))
                return false;

            await _repository.UpdateAsync(specification);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var specification = await _repository.GetByIdAsync(id);
            if (specification == null) return false;

            await _repository.DeleteAsync(specification);
            return true;
        }
    }
}
