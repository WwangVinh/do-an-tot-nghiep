using CoreEntities.Models;
using LogicBusiness.Interfaces.Customer;
using LogicBusiness.Services.Repositories;

namespace LogicBusiness.Services.Customer
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepo;

        public CarService(ICarRepository carRepo)
        {
            _carRepo = carRepo;
        }

        public async Task<object> GetCarsAsync(string? search, string? brand, string? color, decimal? minPrice, decimal? maxPrice, CarStatus? status, string? transmission, string? bodyStyle, int page, int pageSize)
        {
            // 👉 Bơm đủ 10 tham số xuống cho Thủ kho (Repository) làm việc
            var result = await _carRepo.GetCustomerCarsAsync(search, brand, color, minPrice, maxPrice, status, transmission, bodyStyle, page, pageSize);

            // BÍ KÍP Ở ĐÂY: Chỉ lấy đúng những thông tin FE cần để vẽ lên giao diện
            var cleanCars = result.Cars.Select(c => new
            {
                c.CarId,
                c.Name,
                c.Brand,
                c.Year,
                Condition = c.Condition.ToString(),
                c.Price,
                c.ImageUrl,
                Status = c.Status.ToString(),
                c.Mileage,       // Số km đã đi (Odo)
                c.FuelType,      // Xăng / Điện
                c.Transmission,  // Số tự động / Số sàn
                c.BodyStyle      // SUV / Sedan
            });

            return new
            {
                TotalItems = result.TotalCount,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize),
                Data = cleanCars
            };
        }

        // ĐÓNG GÓI DỮ LIỆU (DTO) SIÊU SẠCH CHO KHÁCH HÀNG (Bản Full 2026)
        public async Task<object?> GetCarDetailAsync(int id)
        {
            var car = await _carRepo.GetCarDetailForCustomerAsync(id);

            if (car == null) return null;

            // ĐÓNG GÓI DỮ LIỆU (Bản nâng cấp đầy đủ 2026)
            return new
            {
                car.CarId,
                car.Name,
                car.Brand,
                car.Model,
                car.Year,
                car.Price,
                car.Color,
                car.Mileage,
                car.FuelType,
                car.Transmission, // Số sàn / Số tự động
                car.BodyStyle,    // SUV / Sedan...
                car.Quantity,
                car.Description,
                car.ImageUrl,
                Condition = car.Condition.ToString(),
                Status = car.Status.ToString(),

                // 1. HIỆN THÔNG SỐ (Ní bị thiếu cái này nè!)
                Specifications = car.CarSpecifications
                    .GroupBy(s => s.Category)
                    .Select(group => new {
                        Category = group.Key,
                        Items = group.Select(i => new { i.SpecName, i.SpecValue }).ToList()
                    }).ToList(),

                // 2. TIỆN ÍCH NỔI BẬT (Bản chuẩn cho FE "match" dữ liệu)
                Features = car.CarFeatures
                    .Select(cf => new {
                        cf.FeatureId, // Giữ lại ID để làm key và match logic
                        FeatureName = cf.Feature?.FeatureName,
                        Icon = cf.Feature?.Icon
                    }).ToList(),

                // 3. ALBUM ẢNH
                GalleryImages = car.CarImages
                    .Where(img => img.Is360Degree == false)
                    .GroupBy(img => img.ImageType)
                    .Select(group => new {
                        Category = group.Key,
                        Images = group.Select(i => new { i.Title, i.Description, i.ImageUrl }).ToList()
                    }).ToList(),

                // 4. BỘ ẢNH 360
                Images360 = car.CarImages
                    .Where(img => img.Is360Degree == true)
                    .Select(img => img.ImageUrl)
                    .ToList()
            };
        }
    }
}