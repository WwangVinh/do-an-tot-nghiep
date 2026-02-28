using Microsoft.EntityFrameworkCore;
using LogicBusiness.Interfaces;
using CoreEntities.Models;
using SqlServer.DBContext;
using LogicBusiness.Interfaces.Repositories;

namespace SqlServer.Repositories
{
    public class CarRepository : ICarRepository
    {
        private readonly OtoContext _context;

        public CarRepository(OtoContext context)
        {
            _context = context;
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
                .Include(c => c.CarImages) // 👈 Túm cổ luôn đám ảnh phụ và 360 đi theo
                .FirstOrDefaultAsync(c => c.CarId == id
                                       && c.IsDeleted == false // Chặn xe trong thùng rác
                                       && c.Status != CarStatus.Draft); // Chặn xe bản nháp
        }

        public async Task<Car?> GetCarDetailForAdminAsync(int id)
        {
            return await _context.Cars
                .Include(c => c.CarImages) // Túm cổ toàn bộ ảnh phụ và 360
                .FirstOrDefaultAsync(c => c.CarId == id); // Bắt mọi loại xe
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

        public async Task<bool> CheckNameExistAsync(string name, int excludeId = 0)
        {
            // Tìm xem có xe nào cùng tên (không phân biệt hoa thường) mà khác ID này không
            return await _context.Cars.AnyAsync(c =>
                c.Name.ToLower().Trim() == name.ToLower().Trim() &&
                c.CarId != excludeId);
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

        public async Task DeleteCarAsync(Car car)
        {
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCarAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car); // Lệnh Remove này là xóa bay màu khỏi DB luôn
                await _context.SaveChangesAsync();
            }
        }

        public bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.CarId == id);
        }
    }
}