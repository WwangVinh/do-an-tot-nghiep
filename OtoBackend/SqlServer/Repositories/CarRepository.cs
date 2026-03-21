using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces;
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
            // 1. Khởi tạo câu truy vấn (Chưa chạy xuống DB ngay đâu, đang gom lệnh thôi)
            var query = _context.Cars.AsQueryable();

            // 2. PHÂN QUYỀN HIỂN THỊ CƠ BẢN
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

            // 3. RÁP CÁC ĐIỀU KIỆN LỌC (Cái nào FE truyền lên mới lọc, không thì bỏ qua)

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

            // 4. Chốt đơn: Sắp xếp xe mới nhất lên đầu và lấy dữ liệu
            return await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
        }
        public async Task<(IEnumerable<Car> Cars, int TotalCount)> GetCustomerCarsAsync(string? search, string? brand, string? color, decimal? minPrice, decimal? maxPrice, CarStatus? status, string? transmission, string? bodyStyle, string? fuelType, string? location, int page, int pageSize)
        {
            // Bắt đầu với danh sách toàn bộ xe
            var query = _context.Cars
                                .Include(c => c.CarInventories) // Bước 1: Lấy danh sách kho
                                    .ThenInclude(i => i.Showroom) // Bước 2: QUAN TRỌNG - Phải lấy luôn thông tin Showroom trong kho đó!
                                .AsQueryable();

            // 1. LUÔN LUÔN GIẤU XE RÁC VÀ XE NHÁP
            query = query.Where(c => c.IsDeleted == false);
            query = query.Where(c => c.Status != CarStatus.Draft);

            // 2. LỌC THEO TRẠNG THÁI (Status)
            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            // 3. LỌC THEO TỪ KHÓA (Tìm một phần của Tên xe)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var kw = search.Trim().ToLower();
                // Dùng .Contains() để tìm kiếm tương đối (Ví dụ: gõ "civi" ra "Honda Civic")
                query = query.Where(c => c.Name.ToLower().Contains(kw));
            }

            // 4. LỌC THEO HÃNG XE
            if (!string.IsNullOrWhiteSpace(brand))
            {
                var brandUpper = brand.Trim().ToUpper();
                // Ép về in hoa để so sánh chính xác tuyệt đối (Vì lúc Create mình đã lưu in hoa toàn bộ)
                query = query.Where(c => c.Brand.ToUpper() == brandUpper);
            }

            // 5. LỌC THEO MÀU SẮC
            if (!string.IsNullOrWhiteSpace(color))
            {
                var colorLower = color.Trim().ToLower();
                query = query.Where(c => c.Color.ToLower() == colorLower);
            }

            // 6. LỌC THEO KHOẢNG GIÁ (Giá min / Giá max)
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
            // 🌟 LỌC THEO NHIÊN LIỆU (Xăng, Điện, Dầu...)
            if (!string.IsNullOrWhiteSpace(fuelType))
            {
                query = query.Where(c => c.FuelType != null && c.FuelType.ToLower() == fuelType.Trim().ToLower());
            }

            // 🌟 LỌC THEO ĐỊA CHỈ (Tỉnh/Thành phố)
            // Cực kỳ bá đạo: Chỉ lọc ra những xe đang CÒN HÀNG ở cái Tỉnh/Thành phố mà khách chọn
            if (!string.IsNullOrWhiteSpace(location))
            {
                string loc = location.Trim().ToLower();
                query = query.Where(c => c.CarInventories.Any(inv =>
                    inv.Quantity > 0 &&
                    inv.Showroom != null &&
                    (inv.Showroom.Province.ToLower().Contains(loc) || inv.Showroom.District.ToLower().Contains(loc))
                ));
            }

            // CHỈ HIỆN XE CÒN HÀNG (Quantity > 0)
            query = query.Where(c => c.CarInventories.Any() && c.CarInventories.Sum(i => i.Quantity) > 0);

            // 👉 CHỐT ĐƠN: Đếm tổng số lượng (để chia trang) và cắt lấy dữ liệu
            int totalCount = await query.CountAsync();

            var cars = await query
                .OrderByDescending(c => c.CreatedAt) // Mới nhất lên đầu
                .Skip((page - 1) * pageSize)         // Bỏ qua các xe ở trang trước
                .Take(pageSize)                      // Chỉ lấy đúng số xe của trang hiện tại
                .ToListAsync();

            return (cars, totalCount);
        }

        public async Task<Car?> GetCarDetailForCustomerAsync(int id)
        {
            return await _context.Cars
                .Include(c => c.CarImages) // Lấy album ảnh và ảnh 360
                .Include(c => c.CarSpecifications) // Lấy thông số kỹ thuật (Động cơ, hộp số...)
                .Include(c => c.CarFeatures) // Lấy tiện ích (Cửa sổ trời, Camera 360...)
                    .ThenInclude(cf => cf.Feature) // Lấy chi tiết tên Feature
                .Include(c => c.CarInventories) // 🌟 QUAN TRỌNG: Lấy thông tin tồn kho
                    .ThenInclude(inv => inv.Showroom) // 🌟 QUAN TRỌNG: Lấy tên và địa chỉ Showroom
                .FirstOrDefaultAsync(c => c.CarId == id && c.IsDeleted == false && c.Status != CarStatus.Draft);
        }



        public async Task<Car?> GetCarDetailForAdminAsync(int id)
        {
            return await _context.Cars
                .Include(c => c.CarImages)
                .Include(c => c.CarSpecifications)
                .Include(c => c.CarFeatures)
                    .ThenInclude(cf => cf.Feature)
                .Include(c => c.CarInventories)
                    .ThenInclude(inv => inv.Showroom)
        
                .FirstOrDefaultAsync(c => c.CarId == id);
        }


        public async Task<(IEnumerable<Car> Cars, int TotalCount)> GetAdminCarsAsync(
             string? search, string? brand, string? color,
             decimal? minPrice, decimal? maxPrice, CarStatus? status,
             string? transmission, string? bodyStyle,
             string? fuelType, string? location,
             bool? isDeleted, int page, int pageSize)
        {
            var query = _context.Cars
                                .Include(c => c.CarInventories)
                                    .ThenInclude(i => i.Showroom) // 👈 Phải có ông này thì mới bốc được cái Province ra!
                                .AsQueryable();
            // --- BỘ LỌC MẠNH NHƯ CUSTOMER ---
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

            // --- ĐẶC QUYỀN ADMIN ---
            if (status.HasValue) query = query.Where(c => c.Status == status.Value);
            if (isDeleted.HasValue) query = query.Where(c => c.IsDeleted == isDeleted.Value);

            int totalCount = await query.CountAsync();
            var cars = await query.OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (cars, totalCount);
        }

        //public async Task<IEnumerable<Car>> GetAllCarsAsync()
        //{
        //    return await _context.Cars.ToListAsync();
        //}

        public async Task<Car> GetCarByIdAsync(int id)
        {
            return await _context.Cars.FindAsync(id);
        }

        // Trong CarRepository.cs
        public async Task<bool> CheckCarListingExistAsync(string name, string brand, int? year, string color, int condition, decimal? mileage, int? excludeId = null)
        {
            return await _context.Cars.AnyAsync(c =>
                c.Name.ToLower().Trim() == name.ToLower().Trim() &&
                c.Brand == brand &&
                c.Year == year &&
                c.Color == color &&
                c.Condition == (CarCondition)condition &&
                c.Mileage == (mileage ?? 0m) && // Check Mileage kiểu decimal
                (excludeId == null || c.CarId != excludeId));
        }

        public async Task AddCarAsync(Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
        }

        //public async Task UpdateCarAsync(Car car)
        //{
        //    _context.Entry(car).State = EntityState.Modified;
        //    await _context.SaveChangesAsync();
        //}

        public async Task UpdateCarAsync(Car car)
        {
            // 1. Kiểm tra xem EF có đang "nhớ" (track) phiên bản cũ nào của chiếc xe này không
            var trackedEntity = _context.Cars.Local.FirstOrDefault(c => c.CarId == car.CarId);

            // 2. Nếu có, ra lệnh cho EF "quên" phiên bản cũ đó đi (Detach)
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            // 3. Bây giờ mới an toàn đưa phiên bản mới vào để cập nhật
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

        //public async Task DeleteCarAsync(Car car)
        //{
        //    _context.Cars.Remove(car);
        //    await _context.SaveChangesAsync();
        //}

        public async Task DeleteByCarIdAsync(int carId)
        {
            var features = await _context.CarFeatures.Where(x => x.CarId == carId).ToListAsync();
            if (features.Any())
            {
                _context.CarFeatures.RemoveRange(features);
                await _context.SaveChangesAsync();
            }
        }

        //public async Task DeleteCarAsync(int id)
        //{
        //    var car = await _context.Cars.FindAsync(id);
        //    if (car != null)
        //    {
        //        _context.Cars.Remove(car); // Lệnh Remove này là xóa bay màu khỏi DB luôn
        //        await _context.SaveChangesAsync();
        //    }
        //}

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
    }
}