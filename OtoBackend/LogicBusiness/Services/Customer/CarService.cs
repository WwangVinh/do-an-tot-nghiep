using CoreEntities.Models;
using LogicBusiness.Interfaces.Customer;
using LogicBusiness.Interfaces.Repositories;

namespace LogicBusiness.Services.Customer
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepo;

        public CarService(ICarRepository carRepo)
        {
            _carRepo = carRepo;
        }

        public async Task<object> GetCarsAsync(string? search, string? brand, string? color, decimal? minPrice, decimal? maxPrice, CarStatus? status, int page, int pageSize)
        {
            var result = await _carRepo.GetCustomerCarsAsync(search, brand, color, minPrice, maxPrice, status, page, pageSize);

            // BÍ KÍP Ở ĐÂY: Chỉ lấy đúng những thông tin FE cần để vẽ lên giao diện
            var cleanCars = result.Cars.Select(c => new
            {
                c.CarId,
                c.Name,
                c.Brand,
                c.Price,
                c.ImageUrl,
                Status = c.Status.ToString() // Ép cái số thành chữ cho FE dễ làm nhãn
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

        public async Task<object?> GetCarDetailAsync(int id)
        {
            // Nhờ thủ kho lấy xe + toàn bộ ảnh lên
            var car = await _carRepo.GetCarDetailForCustomerAsync(id);

            // Chặn ngay nếu không tìm thấy (hoặc xe đang nháp/thùng rác)
            if (car == null) return null;

            // ĐÓNG GÓI DỮ LIỆU (DTO) SIÊU SẠCH CHO KHÁCH HÀNG
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
                car.Description,
                car.ImageUrl, // Tấm ảnh Đại diện chính
                Status = car.Status.ToString(),

                // BÍ KÍP: Lọc riêng Album ảnh và GOM NHÓM theo phân loại (Nội thất, Ngoại thất...)
                GalleryImages = car.CarImages
                    .Where(img => img.Is360Degree == false)
                    .GroupBy(img => img.ImageType)
                    .Select(group => new {
                        Category = group.Key, // Tên tab (VD: "Nội thất")
                        // Khách hàng CHỈ CẦN link ảnh để xem, không cần ID để xóa
                        Images = group.Select(i => i.ImageUrl).ToList()
                    }).ToList(),

                // Lọc riêng bộ 360 độ cho FE nạp vào thư viện xoay
                Images360 = car.CarImages
                    .Where(img => img.Is360Degree == true)
                    .Select(img => img.ImageUrl)
                    .ToList()
            };
        }
    }
}