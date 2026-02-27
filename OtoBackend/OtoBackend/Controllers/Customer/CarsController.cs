using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OtoBackend.Interfaces;
using OtoBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtoBackend.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly OtoContext _context;
        private readonly ICarRepository _carRepo;

        public CarsController(OtoContext context, ICarRepository carRepo)
        {
            _context = context;
            _carRepo = carRepo;
        }

        // GET: api/Cars
        [HttpGet]
        public async Task<IActionResult> GetCarsForCustomer(
        [FromQuery] string? search,
        [FromQuery] string? brand,
        [FromQuery] string? color,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
        {
            try
            {
                var result = await _carRepo.GetCustomerCarsAsync(search, brand, color, minPrice, maxPrice, page, pageSize);

                // BÍ KÍP Ở ĐÂY: Chỉ lấy đúng những thông tin FE cần để vẽ lên giao diện
                var cleanCars = result.Cars.Select(c => new
                {
                    c.CarId,
                    c.Name,
                    c.Brand,
                    c.Price,
                    c.ImageUrl,
                    Status = c.Status.ToString() // Ép cái số 1, 2, 3 thành chữ cho dễ đọc
                });

                return Ok(new
                {
                    TotalItems = result.TotalCount,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize),
                    Data = cleanCars // Đưa mảng dữ liệu đã làm sạch vào đây!
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        // GET: api/Cars/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarDetailForCustomer(int id)
        {
            // 1. Nhờ Thủ kho đi tìm đúng chiếc xe có ID này
            var car = await _carRepo.GetCarByIdAsync(id);

            // 2. BẢO VỆ DỮ LIỆU: Chỉ cho khách xem xe nếu nó TỒN TẠI, CHƯA BỊ XÓA và ĐANG BÁN
            if (car == null || car.IsDeleted == true || car.Status != CarStatus.Available)
            {
                return NotFound(new { message = "Chiếc xe này không tồn tại hoặc đã ngừng bán!" });
            }

            // 3. Trả về TOÀN BỘ thông số chi tiết (Không cần giấu diếm nữa)
            return Ok(new
            {
                car.CarId,
                car.Name,
                car.Brand,
                car.Model,
                car.Year,
                car.Price,
                car.Color,
                car.Mileage,       // Số km đã đi
                car.FuelType,      // Loại nhiên liệu (Xăng, Điện...)
                car.Description,   // Bài viết mô tả dài thoòng loòng
                car.ImageUrl,      // Ảnh chính
                Status = car.Status.ToString(), // Trạng thái (Available)

                // MẸO NÂNG CAO: Sau này ní làm thêm bảng Ảnh phụ (CarImages) 
                // hay Bảng Thông số (CarSpecifications) thì cũng lôi ra nhét hết vào đây cho FE show!
            });
        }

        //// PUT: api/Cars/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutCar(int id, Car car)
        //{
        //    if (id != car.CarId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(car).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!CarExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/Cars
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Car>> PostCar(Car car)
        //{
        //    _context.Cars.Add(car);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetCar", new { id = car.CarId }, car);
        //}

        //// DELETE: api/Cars/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteCar(int id)
        //{
        //    var car = await _context.Cars.FindAsync(id);
        //    if (car == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Cars.Remove(car);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool CarExists(int id)
        //{
        //    return _context.Cars.Any(e => e.CarId == id);
        //}
    }
}
