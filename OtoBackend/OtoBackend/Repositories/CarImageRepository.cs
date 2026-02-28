using Microsoft.EntityFrameworkCore;
using OtoBackend.Interfaces;
using OtoBackend.Models; // Đổi lại namespace Models cho đúng với dự án của ní

namespace OtoBackend.Repositories
{
    public class CarImageRepository : ICarImageRepository
    {
        private readonly OtoContext _context;

        public CarImageRepository(OtoContext context)
        {
            _context = context;
        }

        public async Task<List<CarImage>> GetCarImagesAsync(int carId)
        {
            return await _context.CarImages.Where(img => img.CarId == carId).ToListAsync();
        }

        public async Task<List<CarImage>> Get360ImagesAsync(int carId)
        {
            return await _context.CarImages
                .Where(img => img.CarId == carId && img.Is360Degree == true)
                .OrderBy(img => img.CarImageId)
                .ToListAsync();
        }

        public async Task<CarImage> AddCarImageAsync(CarImage carImage)
        {
            _context.CarImages.Add(carImage);
            await _context.SaveChangesAsync();
            return carImage;
        }

        public async Task<CarImage> GetCarImageByIdAsync(int imageId)
        {
            return await _context.CarImages.FindAsync(imageId);
        }

        public async Task DeleteCarImageAsync(int imageId)
        {
            var img = await _context.CarImages.FindAsync(imageId);
            if (img != null)
            {
                _context.CarImages.Remove(img);
                await _context.SaveChangesAsync();
            }
        }
    }
}