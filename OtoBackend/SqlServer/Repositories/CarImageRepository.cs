using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using CoreEntities.Models;
using SqlServer.DBContext;
using LogicBusiness.Services.Repositories;

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

        public async Task<string> UploadImageAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0) return "";

            // 🚀 Tuyệt chiêu: Lấy thư mục gốc của project Web đang chạy
            string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string uploadsFolder = Path.Combine(wwwrootPath, "uploads", folderName);

            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            // Tạo tên file duy nhất
            string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Trả về đường dẫn để lưu vào SQL Server 2022
            return $"/uploads/{folderName}/{fileName}";
        }

        public async Task<bool> UpdateImageDescriptionAsync(int imageId, string? description, string? title)
        {
            var image = await _context.CarImages.FindAsync(imageId);

            // Nếu không tìm thấy ảnh -> báo lỗi
            if (image == null) return false;

            // 👈 THÊM DÒNG NÀY: Chặn không cho sửa nếu là ảnh 360 độ
            if (image.Is360Degree == true) return false;

            // Nếu qua được ải trên thì mới cho sửa
            image.Description = description;
            image.Title = title;

            _context.CarImages.Update(image);
            await _context.SaveChangesAsync();

            return true;
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