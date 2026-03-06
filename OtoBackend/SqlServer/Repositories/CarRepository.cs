using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces;
using LogicBusiness.Services.Repositories;
using Microsoft.EntityFrameworkCore;
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
        public async Task<(IEnumerable<Car> Cars, int TotalCount)> GetCustomerCarsAsync(string? search, string? brand, string? color, decimal? minPrice, decimal? maxPrice, CarStatus? status, int page, int pageSize)
        {
            var query = _context.Cars.AsQueryable();

            // 1. Giấu xe thùng rác
            query = query.Where(c => c.IsDeleted == false);

            // 2. Giấu bản nháp
            query = query.Where(c => c.Status != CarStatus.Draft);

            // 3. LỌC THEO TAB (Nếu FE truyền status 1, 2 hoặc 3 xuống)
            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            // ... (Giữ nguyên các đoạn if lọc theo search, brand, color, giá của ní ở đây) ...

            int totalCount = await query.CountAsync();

            var cars = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (cars, totalCount);
        }

        public async Task<Car?> GetCarDetailForCustomerAsync(int id)
        {
            return await _context.Cars
                .Include(c => c.CarImages)
                .Include(c => c.CarSpecifications) // Phải có cái này mới lấy được thông số
                .Include(c => c.CarFeatures)       // Phải có cái này mới lấy được tiện ích
                    .ThenInclude(cf => cf.Feature) // Lấy sâu vào bảng Feature để lấy FeatureName
                .FirstOrDefaultAsync(c => c.CarId == id && c.IsDeleted == false);
        }

        //public async Task<Car?> GetCarDetailForAdminAsync(int id)
        //{
        //    return await _context.Cars
        //        .Include(c => c.CarImages) // Túm cổ toàn bộ ảnh phụ và 360
        //        .FirstOrDefaultAsync(c => c.CarId == id); // Bắt mọi loại xe
        //}

        public async Task<Car?> GetCarDetailForAdminAsync(int id)
        {
            return await _context.Cars
                .Include(c => c.CarImages) // Đã có
                .Include(c => c.CarSpecifications) // Thêm cái này để lấy "nồi lẩu" thông số
                .Include(c => c.CarFeatures) // Thêm cái này để lấy Tiện ích
                    .ThenInclude(cf => cf.Feature) // Lấy thêm tên Tiện ích từ bảng Features
                .FirstOrDefaultAsync(c => c.CarId == id);
        }

        public async Task<(IEnumerable<Car> Cars, int TotalCount)> GetAdminCarsAsync(string? search, CarStatus? status, bool? isDeleted, int page, int pageSize)
        {
            var query = _context.Cars.AsQueryable();

            // 1. TÌM KIẾM THEO TÊN HOẶC HÃNG XE
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Name.Contains(search) || c.Brand.Contains(search));
            }

            // 2. LỌC THEO TRẠNG THÁI MỞ BÁN (Draft, Available, Out_of_stock...)
            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            // 3. TÍNH NĂNG "THÙNG RÁC": Lọc xe đã xóa hoặc chưa xóa
            // Nếu FE truyền isDeleted = true -> Lấy xe trong thùng rác
            // Nếu FE truyền isDeleted = false -> Lấy xe đang hoạt động bình thường
            // Nếu FE không truyền gì (null) -> Lấy TẤT CẢ
            if (isDeleted.HasValue)
            {
                query = query.Where(c => c.IsDeleted == isDeleted.Value);
            }

            // 4. ĐẾM VÀ PHÂN TRANG
            var totalCount = await query.CountAsync();

            var cars = await query
                .OrderByDescending(c => c.CreatedAt) // Mới nhất xếp lên đầu
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

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
        public async Task<bool> CheckNameExistAsync(string name, int? excludeId = null)
        {
            // Tìm bất kỳ xe nào có tên trùng (đã dọn khoảng trắng và chữ hoa)
            // Điều kiện: Nếu có excludeId (đang Sửa) thì phải khác ID đó. Nếu là null (đang Tạo) thì check hết.
            return await _context.Cars.AnyAsync(c =>
                c.Name.ToLower().Trim() == name.ToLower().Trim() &&
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