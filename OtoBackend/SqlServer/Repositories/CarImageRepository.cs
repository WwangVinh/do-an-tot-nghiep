using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using CoreEntities.Models;
using SqlServer.DBContext;

namespace SqlServer.Repositories
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

        public async Task DeleteAllImagesByCarIdAsync(int carId)
        {
            // Tìm tất cả ảnh phụ + 360 của cái xe này
            var images = await _context.CarImages.Where(img => img.CarId == carId).ToListAsync();

            if (images.Any())
            {
                _context.CarImages.RemoveRange(images); // Xóa sạch sành sanh
                await _context.SaveChangesAsync();
            }
        }
    }
}