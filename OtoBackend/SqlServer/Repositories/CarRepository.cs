using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using SqlServer.DBContext;

namespace SqlServer.Repositories
{
    public class CarRepository : ICarRepository
    {
        private readonly OtoContext _context;

        public CarRepository(OtoContext context)
        {
            _context = context;
        }

        public async Task<List<Car>> GetFilteredCarsAsync(CarFilterDto filter, bool isAdmin = false)
        {
            var query = _context.Cars.AsQueryable();

            if (!isAdmin)
            {
                // Khách hàng: Chỉ thấy xe đang Active và chưa bị xóa mềm
                query = query.Where(c => c.Status == CoreEntities.Models.CarStatus.Available && c.IsDeleted == false);
            }
            else
            {
                // Admin: Nếu có truyền Status thì lọc theo Status (Nháp/Active)
                if (filter.Status.HasValue)
                    query = query.Where(c => (int)c.Status == filter.Status.Value);
            }

            // RÁP CÁC ĐIỀU KIỆN LỌC (Cái nào FE truyền lên mới lọc, không thì bỏ qua)

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                query = query.Where(c => c.Name.Contains(filter.Keyword) || c.Brand.Contains(filter.Keyword));
            }

            if (!string.IsNullOrWhiteSpace(filter.Brand))
            {
                query = query.Where(c => c.Brand == filter.Brand);
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(c => c.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(c => c.Price <= filter.MaxPrice.Value);
            }

            if (filter.Condition.HasValue)
            {
                query = query.Where(c => (int)c.Condition == filter.Condition.Value);
            }

            // Chốt đơn: Sắp xếp xe mới nhất lên đầu và lấy dữ liệu
            return await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
        }
        public async Task<(IEnumerable<Car> Cars, int TotalCount)> GetCustomerCarsAsync(
            string? search, string? brand, string? color,
            decimal? minPrice, decimal? maxPrice, CarStatus? status,
            string? transmission, string? bodyStyle,
            string? fuelType, string? location,
            CarCondition? condition, int? minYear, int? maxYear,
            string? sort, bool inStockOnly,
            int page, int pageSize)
        {
            // Bắt đầu với danh sách toàn bộ xe
            var query = _context.Cars
                                .Include(c => c.CarInventories) 
                                    .ThenInclude(i => i.Showroom) 
                                .AsQueryable();

            query = query.Where(c => c.IsDeleted == false);
            query = query.Where(c => c.Status != CarStatus.Draft);

            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            if (condition.HasValue)
            {
                query = query.Where(c => c.Condition == condition.Value);
            }

            if (minYear.HasValue)
            {
                query = query.Where(c => c.Year != null && c.Year.Value >= minYear.Value);
            }

            if (maxYear.HasValue)
            {
                query = query.Where(c => c.Year != null && c.Year.Value <= maxYear.Value);
            }

            // LỌC THEO TỪ KHÓA (Tìm một phần của Tên xe)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var kw = search.Trim().ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(kw));
            }

            // LỌC THEO HÃNG XE
            if (!string.IsNullOrWhiteSpace(brand))
            {
                var brandUpper = brand.Trim().ToUpper();
                query = query.Where(c => c.Brand.ToUpper() == brandUpper);
            }

            // LỌC THEO MÀU SẮC
            if (!string.IsNullOrWhiteSpace(color))
            {
                var colorLower = color.Trim().ToLower();
                query = query.Where(c => c.Color.ToLower() == colorLower);
            }

            // LỌC THEO KHOẢNG GIÁ (Giá min / Giá max)
            if (minPrice.HasValue)
            {
                query = query.Where(c => c.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(c => c.Price <= maxPrice.Value);
            }
            // LỌC THEO HỘP SỐ
            if (!string.IsNullOrWhiteSpace(transmission))
            {
                query = query.Where(c => c.Transmission == transmission.Trim());
            }

            // LỌC THEO KIỂU DÁNG (Sedan / SUV)
            if (!string.IsNullOrWhiteSpace(bodyStyle))
            {
                query = query.Where(c => c.BodyStyle == bodyStyle.Trim());
            }
            // LỌC THEO NHIÊN LIỆU (Xăng, Điện, Dầu...)
            if (!string.IsNullOrWhiteSpace(fuelType))
            {
                query = query.Where(c => c.FuelType != null && c.FuelType.ToLower() == fuelType.Trim().ToLower());
            }

            // LỌC THEO ĐỊA CHỈ (Tỉnh/Thành phố)
            if (!string.IsNullOrWhiteSpace(location))
            {
                string loc = location.Trim().ToLower();
                query = query.Where(c => c.CarInventories.Any(inv =>
                    inv.Quantity > 0 &&
                    inv.Showroom != null &&
                    (inv.Showroom.Province.ToLower().Contains(loc) || inv.Showroom.District.ToLower().Contains(loc))
                ));
            }

            if (inStockOnly)
            {
                // CHỈ HIỆN XE CÒN HÀNG (Quantity > 0)
                query = query.Where(c => c.CarInventories.Any() && c.CarInventories.Sum(i => i.Quantity) > 0);
            }

            int totalCount = await query.CountAsync();

            query = (sort ?? "").Trim().ToLower() switch
            {
                "price_asc" => query.OrderBy(c => c.Price),
                "price_desc" => query.OrderByDescending(c => c.Price),
                "year_asc" => query.OrderBy(c => c.Year),
                "year_desc" => query.OrderByDescending(c => c.Year),
                _ => query.OrderByDescending(c => c.CreatedAt)
            };

            var safePage = page <= 0 ? 1 : page;
            var safePageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, 100);

            var cars = await query
                .Skip((safePage - 1) * safePageSize)
                .Take(safePageSize)
                .ToListAsync();

            return (cars, totalCount);
        }

        // CHI TIẾT XE DÀNH CHO KHÁCH HÀNG: Chỉ lấy xe chưa bị xóa và không phải là Draft, còn Admin thì có quyền xem tất cả
        public async Task<Car?> GetCarDetailForCustomerAsync(int id)
        {
            return await _context.Cars
                .Include(c => c.CarImages)
                .Include(c => c.CarSpecifications) 
                .Include(c => c.CarFeatures)
                    .ThenInclude(cf => cf.Feature)
                .Include(c => c.CarPricingVersions)
                .Include(c => c.CarInventories)
                    .ThenInclude(inv => inv.Showroom) 
                .FirstOrDefaultAsync(c => c.CarId == id && c.IsDeleted == false && c.Status != CarStatus.Draft);
        }


        // CHI TIẾT XE DÀNH CHO ADMIN: Có quyền xem tất cả, kể cả xe bị xóa hoặc đang ở trạng thái Nháp
        public async Task<Car?> GetCarDetailForAdminAsync(int id)
        {
            return await _context.Cars
                .Include(c => c.CarImages)
                .Include(c => c.CarSpecifications)
                .Include(c => c.CarFeatures)
                    .ThenInclude(cf => cf.Feature)
                .Include(c => c.CarPricingVersions)
                .Include(c => c.CarInventories)
                    .ThenInclude(inv => inv.Showroom)
        
                .FirstOrDefaultAsync(c => c.CarId == id);
        }

        // DANH SÁCH XE DÀNH CHO ADMIN: Có quyền xem tất cả, kể cả xe bị xóa hoặc đang ở trạng thái Nháp, và có thêm bộ lọc theo Showroom (Dành cho Admin của từng Showroom)
        public async Task<(IEnumerable<Car> Cars, int TotalCount)> GetAdminCarsAsync(
             string? search, string? brand, string? color,
             decimal? minPrice, decimal? maxPrice, CarStatus? status,
             string? transmission, string? bodyStyle,
             string? fuelType, string? location,
             bool? isDeleted, int page, int pageSize, int? userShowroomId = null)
        {
            var query = _context.Cars
                                .Include(c => c.CarInventories)
                                    .ThenInclude(i => i.Showroom) 
                                .AsQueryable();

            if (userShowroomId.HasValue)
            {
                query = query.Where(c => c.CarInventories.Any(inv => inv.ShowroomId == userShowroomId.Value));
            }

            if (!string.IsNullOrWhiteSpace(search)) query = query.Where(c => c.Name.ToLower().Contains(search.Trim().ToLower()));
            if (!string.IsNullOrWhiteSpace(brand)) query = query.Where(c => c.Brand.ToUpper() == brand.Trim().ToUpper());
            if (!string.IsNullOrWhiteSpace(color)) query = query.Where(c => c.Color.ToLower() == color.Trim().ToLower());
            if (minPrice.HasValue) query = query.Where(c => c.Price >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(c => c.Price <= maxPrice.Value);
            if (!string.IsNullOrWhiteSpace(transmission)) query = query.Where(c => c.Transmission == transmission.Trim());
            if (!string.IsNullOrWhiteSpace(bodyStyle)) query = query.Where(c => c.BodyStyle == bodyStyle.Trim());
            if (!string.IsNullOrWhiteSpace(fuelType)) query = query.Where(c => c.FuelType != null && c.FuelType.ToLower() == fuelType.Trim().ToLower());

            if (!string.IsNullOrWhiteSpace(location))
            {
                string loc = location.Trim().ToLower();
                query = query.Where(c => c.CarInventories.Any(inv =>
                    inv.Showroom != null &&
                    (inv.Showroom.Province.ToLower().Contains(loc) || inv.Showroom.District.ToLower().Contains(loc))
                ));
            }

            if (status.HasValue) query = query.Where(c => c.Status == status.Value);
            if (isDeleted.HasValue) query = query.Where(c => c.IsDeleted == isDeleted.Value);

            int totalCount = await query.CountAsync();
            var cars = await query.OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (cars, totalCount);
        }


        public async Task<Car> GetCarByIdAsync(int id)
        {
            return await _context.Cars.FindAsync(id);
        }
        public async Task<bool> CheckCarListingExistAsync(string name, string brand, int? year, string color, int condition, decimal? mileage, int? excludeId = null)
        {
            return await _context.Cars.AnyAsync(c =>
                c.Name.ToLower().Trim() == name.ToLower().Trim() &&
                c.Brand == brand &&
                c.Year == year &&
                c.Color == color &&
                c.Condition == (CarCondition)condition &&
                c.Mileage == (mileage ?? 0m) && 
                (excludeId == null || c.CarId != excludeId));
        }

        public async Task<Car?> GetExistingNewCarAsync(string name, string brand, int year)
        {
            return await _context.Cars
                .FirstOrDefaultAsync(c =>
                    c.Name == name &&
                    c.Brand == brand &&
                    c.Year == year &&
                    c.Condition == CarCondition.New && 
                    c.IsDeleted == false);
        }

        public async Task AddCarAsync(Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateCarAsync(Car car)
        {
            // Kiểm tra xem EF có đang "nhớ" (track) phiên bản cũ nào của chiếc xe này không
            var trackedEntity = _context.Cars.Local.FirstOrDefault(c => c.CarId == car.CarId);

            // Nếu có, ra lệnh cho EF "quên" phiên bản cũ đó đi (Detach)
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            // Bây giờ mới an toàn đưa phiên bản mới vào để cập nhật
            _context.Entry(car).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<Car?> GetByIdAsync(int id)
        {
            return await _context.Cars.FindAsync(id);
        }

        public async Task UpdateAsync(Car car)
        {
            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByCarIdAsync(int carId)
        {
            var features = await _context.CarFeatures.Where(x => x.CarId == carId).ToListAsync();
            if (features.Any())
            {
                _context.CarFeatures.RemoveRange(features);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DeleteCarAsync(int id)
        {
            // Tìm con xe trong Database
            var car = await _context.Cars.FindAsync(id);

            if (car != null)
            {
                // Xóa nó khỏi DB và lưu lại
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.CarId == id);
        }

        public async Task<IEnumerable<Car>> SearchMasterCarsAsync(string query)
        {
            return await _context.Cars
                .Where(c => c.Condition == CarCondition.New &&
                            c.IsDeleted == false &&
                            (c.Name.Contains(query) || c.Brand.Contains(query)))
                .ToListAsync();
        }

        public async Task<IEnumerable<Car>> GetLatestCustomerCarsAsync(int limit)
        {
            int take = limit <= 0 ? 6 : Math.Min(limit, 50);

            return await _context.Cars
                .Include(c => c.CarInventories)
                    .ThenInclude(i => i.Showroom)
                .Where(c => c.IsDeleted == false && c.Status != CarStatus.Draft)
                .OrderByDescending(c => c.CreatedAt)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<Car>> GetBestSellingCustomerCarsAsync(int limit)
        {
            int take = limit <= 0 ? 6 : Math.Min(limit, 50);

            // Count "sold" by summing OrderItems.Quantity on paid orders
            // NOTE: if your business rule differs, adjust PaymentStatus values here.
            var bestSellingCarIds = await _context.OrderItems
                .Where(oi => oi.CarId != null && oi.Quantity != null)
                .Join(
                    _context.Orders,
                    oi => oi.OrderId,
                    o => o.OrderId,
                    (oi, o) => new { oi.CarId, oi.Quantity, o.PaymentStatus }
                )
                .Where(x => x.PaymentStatus == "Paid" || x.PaymentStatus == "Completed" || x.PaymentStatus == "Success")
                .GroupBy(x => x.CarId!.Value)
                .Select(g => new { CarId = g.Key, SoldQty = g.Sum(x => x.Quantity ?? 0) })
                .OrderByDescending(x => x.SoldQty)
                .ThenByDescending(x => x.CarId)
                .Take(take)
                .ToListAsync();

            var ids = bestSellingCarIds.Select(x => x.CarId).ToList();
            if (!ids.Any()) return Array.Empty<Car>();

            var cars = await _context.Cars
                .Include(c => c.CarInventories)
                    .ThenInclude(i => i.Showroom)
                .Where(c => ids.Contains(c.CarId) && c.IsDeleted == false && c.Status != CarStatus.Draft)
                .ToListAsync();

            // Preserve ranking order by sold qty
            var rank = bestSellingCarIds.Select((x, idx) => new { x.CarId, idx }).ToDictionary(x => x.CarId, x => x.idx);
            return cars.OrderBy(c => rank.TryGetValue(c.CarId, out var idx) ? idx : int.MaxValue).ToList();
        }

        public async Task<IEnumerable<PricingCarBaseDto>> GetCarsForPricingAsync(string? brand = null)
        {
            var query = _context.Cars
                .AsNoTracking()
                .Where(c => c.IsDeleted == false);

            if (!string.IsNullOrWhiteSpace(brand))
            {
                var normalized = brand.Trim().ToUpper();
                query = query.Where(c => (c.Brand ?? string.Empty).ToUpper() == normalized);
            }

            return await query
                .OrderBy(c => c.Name)
                .Select(c => new PricingCarBaseDto
                {
                    CarId = c.CarId,
                    Name = c.Name,
                    Brand = c.Brand,
                    ImageUrl = c.ImageUrl,
                    IsDeleted = c.IsDeleted
                })
                .ToListAsync();
        }
    }
}