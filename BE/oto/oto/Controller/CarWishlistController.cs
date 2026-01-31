using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oto.MyModels;

namespace oto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarWishlistController : ControllerBase
    {
        private readonly OtoContext _context;

        public CarWishlistController(OtoContext context)
        {
            _context = context;
        }

        // DTO để tránh cycle JSON + trả đúng dữ liệu cần
        public class CarWishlistDto
        {
            public int WishlistId { get; set; }
            public int? UserId { get; set; }
            public int? CarId { get; set; }
            public DateTime? AddedAt { get; set; }
        }

        private static CarWishlistDto ToDto(CarWishlist x) => new CarWishlistDto
        {
            WishlistId = x.WishlistId,
            UserId = x.UserId,
            CarId = x.CarId,
            AddedAt = x.AddedAt
        };

        // 1) GET: api/CarWishlist
        // Danh sách + filter (tìm kiếm)
        // Ví dụ: /api/CarWishlist?userId=1&carId=5
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarWishlistDto>>> GetAll(int? userId, int? carId)
        {
            var query = _context.CarWishlists.AsNoTracking().AsQueryable();

            if (userId.HasValue) query = query.Where(x => x.UserId == userId.Value);
            if (carId.HasValue) query = query.Where(x => x.CarId == carId.Value);

            var data = await query
                .OrderByDescending(x => x.AddedAt)
                .Select(x => ToDto(x))
                .ToListAsync();

            return Ok(data);
        }

        // 2) GET: api/CarWishlist/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CarWishlistDto>> GetById(int id)
        {
            var item = await _context.CarWishlists.AsNoTracking()
                .FirstOrDefaultAsync(x => x.WishlistId == id);

            if (item == null) return NotFound();

            return Ok(ToDto(item));
        }

        // 3) GET: api/CarWishlist/search?userId=1&carId=5
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CarWishlistDto>>> Search(int? userId, int? carId)
        {
            var query = _context.CarWishlists.AsNoTracking().AsQueryable();

            if (userId.HasValue) query = query.Where(x => x.UserId == userId.Value);
            if (carId.HasValue) query = query.Where(x => x.CarId == carId.Value);

            var data = await query
                .OrderByDescending(x => x.AddedAt)
                .Select(x => ToDto(x))
                .ToListAsync();

            return Ok(data);
        }

        // 4) POST: api/CarWishlist
        // multipart/form-data
        [HttpPost]
        public async Task<ActionResult<CarWishlistDto>> Post(
            [FromForm] int userId,
            [FromForm] int carId
        )
        {
            // (Optional) Nếu muốn chặn trùng wishlist (1 user + 1 car)
            bool existed = await _context.CarWishlists.AnyAsync(x => x.UserId == userId && x.CarId == carId);
            if (existed)
                return Conflict("Xe này đã có trong wishlist của user.");

            // (Optional) validate tồn tại user/car
            bool userOk = await _context.Users.AnyAsync(u => u.UserId == userId);
            bool carOk = await _context.Cars.AnyAsync(c => c.CarId == carId);
            if (!userOk) return BadRequest("UserId không tồn tại.");
            if (!carOk) return BadRequest("CarId không tồn tại.");

            var entity = new CarWishlist
            {
                UserId = userId,
                CarId = carId,
                AddedAt = DateTime.UtcNow
            };

            _context.CarWishlists.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.WishlistId }, ToDto(entity));
        }

        // 5) PUT: api/CarWishlist/5
        // multipart/form-data (cho phép sửa userId/carId nếu bạn muốn)
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(
            int id,
            [FromForm] int? userId,
            [FromForm] int? carId
        )
        {
            var entity = await _context.CarWishlists.FirstOrDefaultAsync(x => x.WishlistId == id);
            if (entity == null) return NotFound();

            // Update kiểu "partial" => gửi field nào sửa field đó
            if (userId.HasValue)
            {
                bool userOk = await _context.Users.AnyAsync(u => u.UserId == userId.Value);
                if (!userOk) return BadRequest("UserId không tồn tại.");
                entity.UserId = userId.Value;
            }

            if (carId.HasValue)
            {
                bool carOk = await _context.Cars.AnyAsync(c => c.CarId == carId.Value);
                if (!carOk) return BadRequest("CarId không tồn tại.");
                entity.CarId = carId.Value;
            }

            // Nếu bạn muốn cập nhật lại thời gian khi sửa:
            entity.AddedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 6) DELETE: api/CarWishlist/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.CarWishlists.FindAsync(id);
            if (entity == null) return NotFound();

            _context.CarWishlists.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
