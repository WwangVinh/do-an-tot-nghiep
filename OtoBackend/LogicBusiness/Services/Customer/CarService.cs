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

        public async Task<object> GetCarsAsync(
            string? search, string? brand, string? color,
            decimal? minPrice, decimal? maxPrice, CarStatus? status,
            string? transmission, string? bodyStyle,
            string? fuelType, string? location,
            CarCondition? condition, int? minYear, int? maxYear,
            string? sort, bool inStockOnly,
            int page, int pageSize)
        {
            // LƯU Ý: Ní nên bổ sung logic lọc Status 2,3,4 bên trong CarRepository 
            // để kết quả phân trang (TotalCount) được chính xác nhất.
            var result = await _carRepo.GetCustomerCarsAsync(
                search, brand, color,
                minPrice, maxPrice, status,
                transmission, bodyStyle,
                fuelType, location,
                condition, minYear, maxYear,
                sort, inStockOnly,
                page, pageSize);

            // Chỉ lấy những xe có Status là Available (2), Out_of_stock (3), COMING_SOON (4)
            var allowedStatuses = new[] { CarStatus.Available, CarStatus.Out_of_stock, CarStatus.COMING_SOON };

            var cleanCars = result.Cars
                .Where(c => allowedStatuses.Contains(c.Status ?? (CarStatus)(-1)))
                .Select(c => {

                    int totalQty = c.CarInventories != null ? c.CarInventories.Sum(i => i.Quantity) : 0;
                    string displayLocation = "";
                    string statusStr = c.Status?.ToString() ?? "";

                    // Logic hiển thị vị trí dựa trên trạng thái
                    if (statusStr == "COMING_SOON")
                    {
                        displayLocation = "Sắp về";
                    }
                    else if (statusStr == "Out_of_stock" || totalQty == 0)
                    {
                        displayLocation = "Hết hàng";
                    }
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
                            if (activeLocations.Count > 2) displayLocation += ", ...";
                        }
                        else
                        {
                            displayLocation = "Đang cập nhật vị trí";
                        }
                    }

                    return new
                    {
                        c.CarId,
                        c.Name,
                        c.Brand,
                        c.Year,
                        Condition = c.Condition.ToString(),
                        c.Price,
                        c.ImageUrl,
                        Status = statusStr,
                        c.Mileage,
                        c.FuelType,
                        c.Transmission,
                        c.BodyStyle,
                        TotalQuantity = totalQty,
                        Showrooms = displayLocation
                    };
                });

            return new
            {
                TotalItems = result.TotalCount,
                CurrentPage = page <= 0 ? 1 : page,
                PageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, 100),
                // Lưu ý: Nếu ní lọc ở đây thì TotalPages có thể bị lệch nhẹ nếu trong Repo ní chưa lọc.
                TotalPages = (int)Math.Ceiling(result.TotalCount / (double)(pageSize <= 0 ? 10 : Math.Min(pageSize, 100))),
                Data = cleanCars
            };
        }

        // XEM CHI TIẾT
        public async Task<object?> GetCarDetailAsync(int id)
        {
            var car = await _carRepo.GetCarDetailForCustomerAsync(id);

            // CHẶN CỨNG: Nếu không thấy xe HOẶC xe có Status không được phép (0, 1, 5) -> Trả về null
            if (car == null ||
                (car.Status != CarStatus.Available &&
                 car.Status != CarStatus.Out_of_stock &&
                 car.Status != CarStatus.COMING_SOON))
            {
                return null;
            }

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
                car.Transmission,
                car.BodyStyle,
                TotalQuantity = car.CarInventories != null ? car.CarInventories.Sum(i => i.Quantity) : 0,

                ShowroomDetails = car.CarInventories?
                    .Where(inv => inv.Quantity > 0)
                    .Select(inv => new {
                        ShowroomId = inv.ShowroomId, // Thêm ID để Frontend gửi Booking chuẩn
                        ShowroomName = inv.Showroom?.Name,
                        ShowroomAddress = inv.Showroom?.FullAddress,
                        Quantity = inv.Quantity
                    }).ToList(),

                car.Description,
                car.ImageUrl,
                Condition = car.Condition.ToString(),
                Status = car.Status.ToString(),

                PricingVersions = (car.CarPricingVersions ?? new List<CarPricingVersion>())
                    .Where(v => v.IsActive)
                    .OrderBy(v => v.SortOrder)
                    .Select(v => new
                    {
                        v.PricingVersionId,
                        Name = v.VersionName,
                        v.PriceVnd,
                        v.SortOrder
                    }).ToList(),

                Specifications = car.CarSpecifications
                    .GroupBy(s => s.Category)
                    .Select(group => new {
                        Category = group.Key,
                        Items = group.Select(i => new { i.SpecName, i.SpecValue }).ToList()
                    }).ToList(),

                Features = car.CarFeatures
                    .Select(cf => new {
                        cf.FeatureId,
                        FeatureName = cf.Feature?.FeatureName,
                        Icon = cf.Feature?.Icon
                    }).ToList(),

                GalleryImages = car.CarImages
                    .Where(img => img.Is360Degree == false)
                    .GroupBy(img => img.ImageType)
                    .Select(group => new {
                        Category = group.Key,
                        Images = group.Select(i => new { i.Title, i.Description, i.ImageUrl }).ToList()
                    }).ToList(),

                Images360 = car.CarImages
                    .Where(img => img.Is360Degree == true)
                    .Select(img => img.ImageUrl)
                    .ToList()
            };
        }

        public async Task<IEnumerable<object>> GetLatestCarsAsync(int limit)
        {
            var cars = await _carRepo.GetLatestCustomerCarsAsync(limit);
            var allowedStatuses = new[] { CarStatus.Available, CarStatus.Out_of_stock, CarStatus.COMING_SOON };

            return cars
                .Where(c => allowedStatuses.Contains(c.Status ?? (CarStatus)(-1)))
                .Select(c =>
                {
                    int totalQty = c.CarInventories != null ? c.CarInventories.Sum(i => i.Quantity) : 0;
                    string displayLocation = "";
                    string statusStr = c.Status?.ToString() ?? "";

                    if (statusStr == "COMING_SOON") displayLocation = "Sắp về";
                    else if (statusStr == "Out_of_stock" || totalQty == 0) displayLocation = "Hết hàng";
                    else if (c.CarInventories != null)
                    {
                        var provinces = c.CarInventories
                            .Where(inv => inv.Quantity > 0 && inv.Showroom != null)
                            .Select(inv => inv.Showroom.Province)
                            .Distinct().ToList();
                        displayLocation = string.Join(", ", provinces.Take(2));
                        if (provinces.Count > 2) displayLocation += ", ...";
                    }

                    return new
                    {
                        c.CarId,
                        c.Name,
                        c.Brand,
                        c.Year,
                        Condition = c.Condition.ToString(),
                        c.Price,
                        c.ImageUrl,
                        Status = statusStr,
                        c.Mileage,
                        c.FuelType,
                        c.Transmission,
                        c.BodyStyle,
                        TotalQuantity = totalQty,
                        Showrooms = displayLocation,
                        c.CreatedAt
                    };
                });
        }

        public async Task<IEnumerable<object>> GetBestSellingCarsAsync(int limit)
        {
            var cars = await _carRepo.GetBestSellingCustomerCarsAsync(limit);
            var allowedStatuses = new[] { CarStatus.Available, CarStatus.Out_of_stock, CarStatus.COMING_SOON };

            return cars
                .Where(c => allowedStatuses.Contains(c.Status ?? (CarStatus)(-1)))
                .Select(c =>
                {
                    int totalQty = c.CarInventories != null ? c.CarInventories.Sum(i => i.Quantity) : 0;
                    string displayLocation = "";
                    string statusStr = c.Status?.ToString() ?? "";

                    if (statusStr == "COMING_SOON") displayLocation = "Sắp về";
                    else if (statusStr == "Out_of_stock" || totalQty == 0) displayLocation = "Hết hàng";
                    else if (c.CarInventories != null)
                    {
                        var provinces = c.CarInventories
                            .Where(inv => inv.Quantity > 0 && inv.Showroom != null)
                            .Select(inv => inv.Showroom.Province)
                            .Distinct().ToList();
                        displayLocation = string.Join(", ", provinces.Take(2));
                        if (provinces.Count > 2) displayLocation += ", ...";
                    }

                    return new
                    {
                        c.CarId,
                        c.Name,
                        c.Brand,
                        c.Year,
                        Condition = c.Condition.ToString(),
                        c.Price,
                        c.ImageUrl,
                        Status = statusStr,
                        c.Mileage,
                        c.FuelType,
                        c.Transmission,
                        c.BodyStyle,
                        TotalQuantity = totalQty,
                        Showrooms = displayLocation,
                        c.CreatedAt
                    };
                });
        }
    }
}