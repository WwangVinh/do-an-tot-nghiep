using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;

namespace SqlServer.Repositories
{
    public class CarRepository : ICarRepository
    {
        private readonly OtoContext _context;

        // Status được phép hiển thị cho khách hàng
        private static readonly CarStatus[] AllowedCustomerStatuses = new[]
        {
            CarStatus.Available,
            CarStatus.Out_of_stock,
            CarStatus.COMING_SOON
        };

        public CarRepository(OtoContext context)
        {
            _context = context;
        }

        // ============================================================
        // ADMIN — DANH SÁCH XE CƠ BẢN (theo CarFilterDto)
        // ============================================================
        public async Task<List<Car>> GetFilteredCarsAsync(CarFilterDto filter, bool isAdmin = false)
        {
            var query = _context.Cars.AsQueryable();

            if (!isAdmin)
            {
                // Khách hàng: Chỉ thấy xe đang Active và chưa bị xóa mềm
                query = query.Where(c => c.Status == CarStatus.Available && c.IsDeleted == false);
            }
            else
            {
                // Admin: Nếu có truyền Status thì lọc theo Status (Nháp/Active)
                if (filter.Status.HasValue)
                    query = query.Where(c => (int)c.Status == filter.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var kw = filter.Keyword.Trim();
                query = query.Where(c => c.Name.Contains(kw) || (c.Brand != null && c.Brand.Contains(kw)));
            }

            if (!string.IsNullOrWhiteSpace(filter.Brand))
            {
                query = query.Where(c => c.Brand == filter.Brand);
            }

            if (filter.MinPrice.HasValue)
                query = query.Where(c => c.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(c => c.Price <= filter.MaxPrice.Value);

            if (filter.Condition.HasValue)
                query = query.Where(c => (int)c.Condition == filter.Condition.Value);

            return await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
        }

        // ============================================================
        // CUSTOMER — DANH SÁCH XE (có phân trang, filter, sort)
        // ============================================================
        public async Task<(IEnumerable<Car> Cars, int TotalCount)> GetCustomerCarsAsync(
            string? search, string? brand, string? color,
            decimal? minPrice, decimal? maxPrice, CarStatus? status,
            string? transmission, string? bodyStyle,
            string? fuelType, string? location,
            CarCondition? condition, int? minYear, int? maxYear,
            string? sort, bool inStockOnly,
            int page, int pageSize)
        {
            var query = _context.Cars
                .Include(c => c.CarInventories)
                    .ThenInclude(i => i.Showroom)
                .Include(c => c.CarColors)
                .AsQueryable();

            // ===== HARD FILTER cho customer =====
            query = query.Where(c => c.IsDeleted == false);

            // CHỈ HIỆN xe có Status thuộc danh sách cho phép (Available, Out_of_stock, COMING_SOON)
            // Trước đây chỉ loại Draft → vẫn lọt Pending/Rejected/Inactive → khách thấy bậy
            query = query.Where(c => c.Status.HasValue && AllowedCustomerStatuses.Contains(c.Status.Value));

            // Nếu FE truyền status cụ thể (vd chỉ xem xe Available) thì filter thêm
            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            if (condition.HasValue)
                query = query.Where(c => c.Condition == condition.Value);

            if (minYear.HasValue)
                query = query.Where(c => c.Year != null && c.Year.Value >= minYear.Value);

            if (maxYear.HasValue)
                query = query.Where(c => c.Year != null && c.Year.Value <= maxYear.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var kw = search.Trim().ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(kw));
            }

            if (!string.IsNullOrWhiteSpace(brand))
            {
                var brandUpper = brand.Trim().ToUpper();
                query = query.Where(c => c.Brand != null && c.Brand.ToUpper() == brandUpper);
            }

            // LỌC THEO MÀU (qua bảng CarColors)
            if (!string.IsNullOrWhiteSpace(color))
            {
                var colorLower = color.Trim().ToLower();
                query = query.Where(c => c.CarColors.Any(cc =>
                    cc.IsActive && cc.ColorName.ToLower() == colorLower));
            }

            if (minPrice.HasValue)
                query = query.Where(c => c.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(c => c.Price <= maxPrice.Value);

            if (!string.IsNullOrWhiteSpace(transmission))
                query = query.Where(c => c.Transmission == transmission.Trim());

            if (!string.IsNullOrWhiteSpace(bodyStyle))
                query = query.Where(c => c.BodyStyle == bodyStyle.Trim());

            if (!string.IsNullOrWhiteSpace(fuelType))
            {
                var fuelLower = fuelType.Trim().ToLower();
                query = query.Where(c => c.FuelType != null && c.FuelType.ToLower() == fuelLower);
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                string loc = location.Trim().ToLower();
                query = query.Where(c => c.CarInventories.Any(inv =>
                    inv.Quantity > 0 &&
                    inv.Showroom != null &&
                    ((inv.Showroom.Province != null && inv.Showroom.Province.ToLower().Contains(loc))
                     || (inv.Showroom.District != null && inv.Showroom.District.ToLower().Contains(loc)))
                ));
            }

            if (inStockOnly)
            {
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

        // ============================================================
        // CUSTOMER — CHI TIẾT XE (Include CarColor cho ShowroomDetails)
        // ============================================================
        public async Task<Car?> GetCarDetailForCustomerAsync(int id)
        {
            return await _context.Cars
                .AsNoTracking()
                .Include(c => c.CarImages)
                .Include(c => c.CarColors)
                .Include(c => c.CarSpecifications)
                .Include(c => c.CarFeatures)
                    .ThenInclude(cf => cf.Feature)
                .Include(c => c.CarPricingVersions)
                .Include(c => c.CarInventories)
                    .ThenInclude(inv => inv.Showroom)
                .Include(c => c.CarInventories)
                    .ThenInclude(inv => inv.CarColor) // ← THÊM: để FE biết lô hàng đó là màu gì
                .FirstOrDefaultAsync(c =>
                    c.CarId == id
                    && c.IsDeleted == false
                    && c.Status.HasValue
                    && AllowedCustomerStatuses.Contains(c.Status.Value));
        }

        // ============================================================
        // ADMIN — CHI TIẾT XE (xem tất cả, kể cả Draft / IsDeleted)
        // ============================================================
        public async Task<Car?> GetCarDetailForAdminAsync(int id)
        {
            return await _context.Cars
                .Include(c => c.CarImages)
                .Include(c => c.CarColors)
                .Include(c => c.CarSpecifications)
                .Include(c => c.CarFeatures)
                    .ThenInclude(cf => cf.Feature)
                .Include(c => c.CarPricingVersions)
                .Include(c => c.CarInventories)
                    .ThenInclude(inv => inv.Showroom)
                .Include(c => c.CarInventories)
                    .ThenInclude(inv => inv.CarColor)
                .FirstOrDefaultAsync(c => c.CarId == id);
        }

        // ============================================================
        // ADMIN — DANH SÁCH XE (xem tất cả, lọc theo showroom)
        // ============================================================
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
                .Include(c => c.CarColors) // ← THÊM: để filter color join được, và FE hiển thị
                .AsQueryable();

            if (userShowroomId.HasValue)
            {
                query = query.Where(c => c.CarInventories.Any(inv => inv.ShowroomId == userShowroomId.Value));
            }

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c => c.Name.ToLower().Contains(search.Trim().ToLower()));

            if (!string.IsNullOrWhiteSpace(brand))
                query = query.Where(c => c.Brand != null && c.Brand.ToUpper() == brand.Trim().ToUpper());

            if (!string.IsNullOrWhiteSpace(color))
            {
                var colorLower = color.Trim().ToLower();
                query = query.Where(c => c.CarColors.Any(cc => cc.ColorName.ToLower() == colorLower));
            }

            if (minPrice.HasValue) query = query.Where(c => c.Price >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(c => c.Price <= maxPrice.Value);
            if (!string.IsNullOrWhiteSpace(transmission)) query = query.Where(c => c.Transmission == transmission.Trim());
            if (!string.IsNullOrWhiteSpace(bodyStyle)) query = query.Where(c => c.BodyStyle == bodyStyle.Trim());

            if (!string.IsNullOrWhiteSpace(fuelType))
            {
                var fuelLower = fuelType.Trim().ToLower();
                query = query.Where(c => c.FuelType != null && c.FuelType.ToLower() == fuelLower);
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                string loc = location.Trim().ToLower();
                query = query.Where(c => c.CarInventories.Any(inv =>
                    inv.Showroom != null &&
                    ((inv.Showroom.Province != null && inv.Showroom.Province.ToLower().Contains(loc))
                     || (inv.Showroom.District != null && inv.Showroom.District.ToLower().Contains(loc)))
                ));
            }

            if (status.HasValue) query = query.Where(c => c.Status == status.Value);
            if (isDeleted.HasValue) query = query.Where(c => c.IsDeleted == isDeleted.Value);

            int totalCount = await query.CountAsync();

            var safePage = page <= 0 ? 1 : page;
            var safePageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, 100);

            var cars = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((safePage - 1) * safePageSize)
                .Take(safePageSize)
                .ToListAsync();

            return (cars, totalCount);
        }

        // ============================================================
        // GET BY ID — Có 2 method với mục đích khác nhau
        // ============================================================

        /// <summary>
        /// GetByIdAsync — DÙNG CHO VALIDATION (vd CarInventoryService validate màu).
        /// Có Include(CarColors) để service kiểm tra màu thuộc về xe.
        /// </summary>
        public async Task<Car?> GetByIdAsync(int id)
        {
            return await _context.Cars
                .Include(c => c.CarColors)
                .FirstOrDefaultAsync(c => c.CarId == id);
        }

        /// <summary>
        /// GetCarByIdAsync — Lấy xe thô, không Include gì.
        /// Dùng khi chỉ cần info cơ bản.
        /// </summary>
        public async Task<Car?> GetCarByIdAsync(int id)
        {
            return await _context.Cars.FindAsync(id);
        }

        // ============================================================
        // CHECK / SEARCH
        // ============================================================
        public async Task<bool> CheckCarListingExistAsync(string name, string brand, int? year, int condition, decimal? mileage, int? excludeId = null)
        {
            var nameNorm = name.ToLower().Trim();
            var brandNorm = brand.Trim();
            var carCondition = (CarCondition)condition;

            return await _context.Cars.AnyAsync(c =>
                c.Name.ToLower().Trim() == nameNorm &&
                c.Brand == brandNorm &&
                c.Year == year &&
                c.Condition == carCondition &&
                // FIX: so sánh nullable đúng cách — null khớp null, value khớp value
                ((c.Mileage == null && mileage == null) || (c.Mileage == mileage)) &&
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

        public bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.CarId == id);
        }

        public async Task<IEnumerable<Car>> SearchMasterCarsAsync(string query)
        {
            return await _context.Cars
                .Where(c => c.Condition == CarCondition.New &&
                            c.IsDeleted == false &&
                            (c.Name.Contains(query) || (c.Brand != null && c.Brand.Contains(query))))
                .ToListAsync();
        }

        // ============================================================
        // CUD
        // ============================================================
        public async Task AddCarAsync(Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCarAsync(Car car)
        {
            // Detach phiên bản cũ EF đang track (nếu có)
            var trackedEntity = _context.Cars.Local.FirstOrDefault(c => c.CarId == car.CarId);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            // ⚠️ LƯU Ý: State = Modified sẽ update TOÀN BỘ field.
            // Nếu service không gán đầy đủ field (vd quên CreatedByUserId, CreatedAt)
            // thì các field đó sẽ bị overwrite về default!
            // → Service phải load entity gốc trước, modify rồi mới gọi method này,
            //   hoặc service nên dùng pattern attach + chỉ mark Modified field cần đổi.
            _context.Entry(car).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Car car)
        {
            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// XÓA CarFeatures của một xe (không xóa Car).
        /// ⚠️ Tên cũ "DeleteByCarIdAsync" gây hiểu nhầm là xóa Car.
        /// Giữ tên cũ để backward compat nhưng có comment rõ.
        /// </summary>
        public async Task DeleteByCarIdAsync(int carId)
        {
            var features = await _context.CarFeatures.Where(x => x.CarId == carId).ToListAsync();
            if (features.Any())
            {
                _context.CarFeatures.RemoveRange(features);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// SOFT DELETE — đánh dấu IsDeleted = true thay vì xóa cứng.
        /// Tránh vỡ FK với Bookings, Orders, OrderItems...
        /// </summary>
        public async Task<bool> DeleteCarAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return false;

            // FIX: chuyển từ HARD delete sang SOFT delete để giữ tham chiếu lịch sử
            car.IsDeleted = true;
            car.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // ============================================================
        // CUSTOMER — XE MỚI NHẤT / BÁN CHẠY (đã filter Status hợp lệ)
        // ============================================================
        public async Task<IEnumerable<Car>> GetLatestCustomerCarsAsync(int limit)
        {
            int take = limit <= 0 ? 6 : Math.Min(limit, 50);

            return await _context.Cars
                .Include(c => c.CarInventories)
                    .ThenInclude(i => i.Showroom)
                .Include(c => c.CarColors)
                .Where(c => c.IsDeleted == false
                            && c.Status.HasValue
                            && AllowedCustomerStatuses.Contains(c.Status.Value))
                .OrderByDescending(c => c.CreatedAt)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<Car>> GetBestSellingCustomerCarsAsync(int limit)
        {
            int take = limit <= 0 ? 6 : Math.Min(limit, 50);

            // Tính SoldQty từ OrderItems trên các đơn đã thanh toán
            var bestSellingCarIds = await _context.OrderItems
                .Where(oi => oi.CarId != null && oi.Quantity != null)
                .Join(
                    _context.Orders,
                    oi => oi.OrderId,
                    o => o.OrderId,
                    (oi, o) => new { oi.CarId, oi.Quantity, o.PaymentStatus }
                )
                .Where(x => x.PaymentStatus == "Paid"
                            || x.PaymentStatus == "Completed"
                            || x.PaymentStatus == "Success")
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
                .Include(c => c.CarColors)
                .Where(c => ids.Contains(c.CarId)
                            && c.IsDeleted == false
                            && c.Status.HasValue
                            && AllowedCustomerStatuses.Contains(c.Status.Value))
                .ToListAsync();

            // Giữ thứ tự rank theo sold qty
            var rank = bestSellingCarIds
                .Select((x, idx) => new { x.CarId, idx })
                .ToDictionary(x => x.CarId, x => x.idx);

            return cars
                .OrderBy(c => rank.TryGetValue(c.CarId, out var idx) ? idx : int.MaxValue)
                .ToList();
        }

        // ============================================================
        // PRICING
        // ============================================================
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