using CoreEntities.Models;
using LogicBusiness.Interfaces.Customer;
using LogicBusiness.Interfaces.Repositories;

namespace LogicBusiness.Services.Customer
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepo;

        // Status được phép hiển thị cho khách hàng.
        // Repo đã filter ở SQL — đây là lớp defense-in-depth phòng khi Repo bị sửa.
        private static readonly CarStatus[] AllowedCustomerStatuses = new[]
        {
            CarStatus.Available,
            CarStatus.Out_of_stock,
            CarStatus.COMING_SOON
        };

        public CarService(ICarRepository carRepo)
        {
            _carRepo = carRepo;
        }

        // ==========================================================
        // GET LIST (search/filter/paginate)
        // ==========================================================
        public async Task<object> GetCarsAsync(
            string? search, string? brand, string? color,
            decimal? minPrice, decimal? maxPrice, CarStatus? status,
            string? transmission, string? bodyStyle,
            string? fuelType, string? location,
            CarCondition? condition, int? minYear, int? maxYear,
            string? sort, bool inStockOnly,
            int page, int pageSize)
        {
            // Repo đã handle filter Status, color, paging chuẩn → TotalCount chính xác
            var result = await _carRepo.GetCustomerCarsAsync(
                search, brand, color,
                minPrice, maxPrice, status,
                transmission, bodyStyle,
                fuelType, location,
                condition, minYear, maxYear,
                sort, inStockOnly,
                page, pageSize);

            // Defense-in-depth: lọc lại 1 lần nữa ở Service phòng Repo bug
            var cleanCars = result.Cars
                .Where(c => c.Status.HasValue && AllowedCustomerStatuses.Contains(c.Status.Value))
                .Select(MapToCustomerListDto);

            int safePageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, 100);

            return new
            {
                TotalItems = result.TotalCount,
                CurrentPage = page <= 0 ? 1 : page,
                PageSize = safePageSize,
                TotalPages = (int)Math.Ceiling(result.TotalCount / (double)safePageSize),
                Data = cleanCars
            };
        }

        // ==========================================================
        // GET DETAIL
        // ==========================================================
        public async Task<object?> GetCarDetailAsync(int id)
        {
            var car = await _carRepo.GetCarDetailForCustomerAsync(id);

            // Repo đã filter sẵn nhưng vẫn check defensive
            if (car == null
                || !car.Status.HasValue
                || !AllowedCustomerStatuses.Contains(car.Status.Value))
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

                // Chỉ trả màu đang active, fallback ảnh màu → ảnh chính nếu null
                Colors = car.CarColors?
                    .Where(cc => cc.IsActive)
                    .Select(cc => new
                    {
                        cc.CarColorId,
                        cc.ColorName,
                        cc.HexCode,
                        ImageUrl = string.IsNullOrWhiteSpace(cc.ImageUrl) ? car.ImageUrl : cc.ImageUrl
                    }).ToList(),

                car.Mileage,
                car.FuelType,
                car.Transmission,
                car.BodyStyle,
                TotalQuantity = car.CarInventories?.Sum(i => i.Quantity) ?? 0,

                // Trả thêm CarColorId/ColorName cho mỗi lô hàng để FE hiện
                // "Đỏ — 3 chiếc tại Hà Nội", "Đen — 1 chiếc tại HCM"
                ShowroomDetails = car.CarInventories?
                    .Where(inv => inv.Quantity > 0)
                    .Select(inv => new
                    {
                        ShowroomId = inv.ShowroomId,
                        ShowroomName = inv.Showroom?.Name,
                        ShowroomAddress = inv.Showroom?.FullAddress,
                        Quantity = inv.Quantity,
                        CarColorId = inv.CarColorId,
                        ColorName = inv.CarColor?.ColorName,
                        HexCode = inv.CarColor?.HexCode
                    }).ToList(),

                car.Description,
                car.ImageUrl,
                Condition = car.Condition.ToString(),
                Status = car.Status?.ToString() ?? "",

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
                    .Select(group => new
                    {
                        Category = group.Key,
                        Items = group.Select(i => new { i.SpecName, i.SpecValue }).ToList()
                    }).ToList(),

                Features = car.CarFeatures
                    .Select(cf => new
                    {
                        cf.FeatureId,
                        FeatureName = cf.Feature?.FeatureName,
                        Icon = cf.Feature?.Icon
                    }).ToList(),

                GalleryImages = car.CarImages
                    .Where(img => img.Is360Degree == false)
                    .GroupBy(img => img.ImageType)
                    .Select(group => new
                    {
                        Category = group.Key,
                        Images = group.Select(i => new { i.Title, i.Description, i.ImageUrl }).ToList()
                    }).ToList(),

                Images360 = car.CarImages
                    .Where(img => img.Is360Degree == true)
                    .Select(img => img.ImageUrl)
                    .ToList()
            };
        }

        // ==========================================================
        // GET LATEST
        // ==========================================================
        public async Task<IEnumerable<object>> GetLatestCarsAsync(int limit)
        {
            var cars = await _carRepo.GetLatestCustomerCarsAsync(limit);
            return cars
                .Where(c => c.Status.HasValue && AllowedCustomerStatuses.Contains(c.Status.Value))
                .Select(MapToCustomerListDtoWithCreatedAt);
        }

        // ==========================================================
        // GET BEST SELLING
        // ==========================================================
        public async Task<IEnumerable<object>> GetBestSellingCarsAsync(int limit)
        {
            var cars = await _carRepo.GetBestSellingCustomerCarsAsync(limit);
            return cars
                .Where(c => c.Status.HasValue && AllowedCustomerStatuses.Contains(c.Status.Value))
                .Select(MapToCustomerListDtoWithCreatedAt);
        }

        // ==========================================================
        // PRIVATE HELPERS
        // ==========================================================

        /// <summary>
        /// Build chuỗi hiển thị vị trí (Sắp về / Hết hàng / Hà Nội, HCM, ...).
        /// </summary>
        private static string BuildDisplayLocation(Car c, int totalQty)
        {
            if (c.Status == CarStatus.COMING_SOON) return "Sắp về";
            if (c.Status == CarStatus.Out_of_stock || totalQty == 0) return "Hết hàng";

            if (c.CarInventories == null) return "Đang cập nhật vị trí";

            var activeLocations = c.CarInventories
                .Where(inv => inv.Quantity > 0
                              && inv.Showroom != null
                              && !string.IsNullOrWhiteSpace(inv.Showroom.Province))
                .Select(inv => inv.Showroom.Province)
                .Distinct()
                .ToList();

            if (!activeLocations.Any()) return "Đang cập nhật vị trí";

            string result = string.Join(", ", activeLocations.Take(2));
            if (activeLocations.Count > 2) result += ", ...";
            return result;
        }

        /// <summary>Map cho list view chính (search/filter) — không có CreatedAt.</summary>
        private static object MapToCustomerListDto(Car c)
        {
            int totalQty = c.CarInventories?.Sum(i => i.Quantity) ?? 0;
            return new
            {
                c.CarId,
                c.Name,
                c.Brand,
                c.Year,
                Condition = c.Condition.ToString(),
                c.Price,
                Colors = c.CarColors?
                    .Where(cc => cc.IsActive)
                    .Select(cc => new
                    {
                        cc.CarColorId,
                        cc.ColorName,
                        cc.HexCode,
                        ImageUrl = string.IsNullOrWhiteSpace(cc.ImageUrl) ? c.ImageUrl : cc.ImageUrl
                    }).ToList(),
                c.ImageUrl,
                Status = c.Status?.ToString() ?? "",
                c.Mileage,
                c.FuelType,
                c.Transmission,
                c.BodyStyle,
                TotalQuantity = totalQty,
                Showrooms = BuildDisplayLocation(c, totalQty)
            };
        }

        /// <summary>Map cho Latest/BestSelling — có thêm CreatedAt.</summary>
        private static object MapToCustomerListDtoWithCreatedAt(Car c)
        {
            int totalQty = c.CarInventories?.Sum(i => i.Quantity) ?? 0;
            return new
            {
                c.CarId,
                c.Name,
                c.Brand,
                c.Year,
                Condition = c.Condition.ToString(),
                c.Price,
                Colors = c.CarColors?
                    .Where(cc => cc.IsActive)
                    .Select(cc => new
                    {
                        cc.CarColorId,
                        cc.ColorName,
                        cc.HexCode,
                        ImageUrl = string.IsNullOrWhiteSpace(cc.ImageUrl) ? c.ImageUrl : cc.ImageUrl
                    }).ToList(),
                c.ImageUrl,
                Status = c.Status?.ToString() ?? "",
                c.Mileage,
                c.FuelType,
                c.Transmission,
                c.BodyStyle,
                TotalQuantity = totalQty,
                Showrooms = BuildDisplayLocation(c, totalQty),
                c.CreatedAt
            };
        }
    }
}