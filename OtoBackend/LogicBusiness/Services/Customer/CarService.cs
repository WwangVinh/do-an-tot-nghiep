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

        // 👉 Bơm thêm fuelType và location vào đây nè
        public async Task<object> GetCarsAsync(string? search, string? brand, string? color, decimal? minPrice, decimal? maxPrice, CarStatus? status, string? transmission, string? bodyStyle, string? fuelType, string? location, int page, int pageSize)
        {
            // Truyền đủ xuống Repo
            var result = await _carRepo.GetCustomerCarsAsync(search, brand, color, minPrice, maxPrice, status, transmission, bodyStyle, fuelType, location, page, pageSize);

            var cleanCars = result.Cars.Select(c => {

                int totalQty = c.CarInventories != null ? c.CarInventories.Sum(i => i.Quantity) : 0;
                string displayLocation = ""; // Khởi tạo rỗng trước

                // 1. ƯU TIÊN 1: Check trạng thái xe trước
                if (c.Status.ToString() == "ComingSoon") // Ní sửa lại thành tên Enum của ní cho đúng nhé (VD: CarStatus.ComingSoon)
                {
                    displayLocation = "Sắp về";
                }
                // 2. ƯU TIÊN 2: Nếu xe đang bán nhưng hết tồn kho
                else if (totalQty == 0)
                {
                    displayLocation = "Hết hàng";
                }
                // 3. ƯU TIÊN 3: Có hàng trong kho
                else if (c.CarInventories != null)
                {
                    var activeLocations = c.CarInventories
                        .Where(inv => inv.Quantity > 0 && inv.Showroom != null && !string.IsNullOrWhiteSpace(inv.Showroom.Province))
                        .Select(inv => inv.Showroom.Province)
                        .Distinct()
                        .ToList();

                    if (activeLocations.Any())
                    {
                        displayLocation = string.Join(", ", activeLocations.Take(2));
                        if (activeLocations.Count > 2)
                        {
                            displayLocation += ", ...";
                        }
                    }
                    else
                    {
                        displayLocation = "Đang cập nhật vị trí"; // Lỡ kho có xe mà quên gắn Showroom
                    }
                }

                // Trả về dữ liệu sạch sẽ cho Frontend
                return new
                {
                    c.CarId,
                    c.Name,
                    c.Brand,
                    c.Year,
                    Condition = c.Condition.ToString(),
                    c.Price,
                    c.ImageUrl,
                    Status = c.Status.ToString(), // Cái này Frontend có thể dùng để hiện Label màu (Đỏ = Hết hàng, Xanh = Đang bán, Vàng = Sắp về)
                    c.Mileage,
                    c.FuelType,
                    c.Transmission,
                    c.BodyStyle,
                    TotalQuantity = totalQty,
                    Showrooms = displayLocation // Bây giờ nó sẽ cực kỳ thông minh
                };
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
                TotalQuantity = car.CarInventories != null ? car.CarInventories.Sum(i => i.Quantity) : 0,

                // ✅ THÊM VÀO ĐÂY: Trả về danh sách Showroom đang CÒN xe này
                ShowroomDetails = car.CarInventories?
                    .Where(inv => inv.Quantity > 0)
                    .Select(inv => new {
                        ShowroomName = inv.Showroom?.Name,
                        ShowroomAddress = inv.Showroom?.FullAddress, // 👈 Gọi cái Getter ở Model ní mới làm
                        Quantity = inv.Quantity
                    }).ToList(),

                car.Description,
                car.ImageUrl,
                Condition = car.Condition.ToString(),
                Status = car.Status.ToString(),

                // 1. HIỆN THÔNG SỐ
                Specifications = car.CarSpecifications
                    .GroupBy(s => s.Category)
                    .Select(group => new {
                        Category = group.Key,
                        Items = group.Select(i => new { i.SpecName, i.SpecValue }).ToList()
                    }).ToList(),

                // 2. TIỆN ÍCH NỔI BẬT
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