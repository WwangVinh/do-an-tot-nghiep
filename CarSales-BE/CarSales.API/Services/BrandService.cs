using CarSales.API.Models;
using CarSales.API.Repositories;

namespace CarSales.API.Services     //Bảng Hãng xe (Brands) phần thực hiện
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _repository;

        public BrandService(IBrandRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Brand>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Brand?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Brand> CreateAsync(Brand brand)
        {
            await _repository.AddAsync(brand);
            return brand;
        }

        public async Task<bool> UpdateAsync(int id, Brand brand)
        {
            if (id != brand.BrandId || !_repository.Exists(id))
                return false;

            await _repository.UpdateAsync(brand);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var brand = await _repository.GetByIdAsync(id);
            if (brand == null) return false;

            await _repository.DeleteAsync(brand);
            return true;
        }
    }
}
