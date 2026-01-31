using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oto.MyModels;

namespace oto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly OtoContext _context;

        public CarController(OtoContext context)
        {
            _context = context;
        }

        // 1. GET: api/Car
        // Lấy danh sách tất cả xe
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> GetCars()
        {
            var cars = await _context.Cars.ToListAsync();
            return Ok(cars);
        }

        // 2. GET: api/Car/5
        // Lấy thông tin chi tiết của một xe theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Car>> GetCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);

            if (car == null)
            {
                return NotFound();
            }

            return car;
        }

        // 3. POST: api/Car
        // Thêm mới một xe
        [HttpPost]
        public async Task<ActionResult<Car>> PostCar(
     [FromForm] string name,
     [FromForm] string? model,
     [FromForm] int? year,
     [FromForm] decimal? price,
     [FromForm] string? color,
     [FromForm] string? description,
     [FromForm] string? brand,
     [FromForm] decimal? mileage,
     [FromForm] string? imageUrl,
     [FromForm] string? status)
        {
            if (string.IsNullOrEmpty(name) || price == null)
            {
                return BadRequest("Tên xe và giá là bắt buộc.");  // Trả về lỗi nếu thiếu name hoặc price
            }

            var car = new Car
            {
                Name = name,
                Model = model,
                Year = year,
                Price = price,
                Color = color,
                Description = description,
                Brand = brand,
                Mileage = mileage,
                ImageUrl = imageUrl,
                Status = status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCar), new { id = car.CarId }, car);
        }


        // 4. PUT: api/Car/5
        // Cập nhật thông tin xe theo ID
        // 4. PUT: api/Car/5
        // Cập nhật thông tin xe theo ID
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCar(int id,
    [FromForm] string? name,
    [FromForm] string? model,
    [FromForm] int? year,
    [FromForm] decimal? price,
    [FromForm] string? color,
    [FromForm] string? description,
    [FromForm] string? brand,
    [FromForm] decimal? mileage,
    [FromForm] string? imageUrl,
    [FromForm] string? status)
        {
            var car = await _context.Cars.FindAsync(id);

            if (car == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy xe
            }

            // Cập nhật các trường với dữ liệu mới, giữ nguyên các trường không thay đổi
            car.Name = string.IsNullOrEmpty(name) ? car.Name : name;
            car.Model = string.IsNullOrEmpty(model) ? car.Model : model;
            car.Year = year ?? car.Year;
            car.Price = price ?? car.Price;
            car.Color = string.IsNullOrEmpty(color) ? car.Color : color;
            car.Description = string.IsNullOrEmpty(description) ? car.Description : description;
            car.Brand = string.IsNullOrEmpty(brand) ? car.Brand : brand;
            car.Mileage = mileage ?? car.Mileage;
            car.ImageUrl = string.IsNullOrEmpty(imageUrl) ? car.ImageUrl : imageUrl;
            car.Status = string.IsNullOrEmpty(status) ? car.Status : status;
            car.UpdatedAt = DateTime.UtcNow; // Cập nhật thời gian sửa

            _context.Entry(car).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarExists(id))
                {
                    return NotFound();  // Trả về 404 nếu không tìm thấy xe
                }
                else
                {
                    throw;
                }
            }

            return NoContent();  // Trả về thành công sau khi cập nhật
        }


        // 5. DELETE: api/Car/5
        // Xóa xe theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound(); // Nếu không tìm thấy xe, trả về 404 Not Found
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return NoContent(); // Trả về thành công sau khi xóa
        }

        // Kiểm tra nếu xe có tồn tại trong cơ sở dữ liệu
        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.CarId == id);
        }
    }
}
