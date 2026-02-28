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
        private readonly ICarImageRepository _imageRepo;

        public CarsController(OtoContext context, ICarRepository carRepo, ICarImageRepository imageRepo)
        {
            _context = context;
            _carRepo = carRepo;
            _imageRepo = imageRepo;
        }

        //// GET: api/Cars
        //[HttpGet]
        //public async Task<IActionResult> GetCarsForCustomer(
        //[FromQuery] string? search,
        //[FromQuery] string? brand,
        //[FromQuery] string? color,
        //[FromQuery] decimal? minPrice,
        //[FromQuery] decimal? maxPrice,
        //[FromQuery] int page = 1,
        //[FromQuery] int pageSize = 12)
        //{
        //    try
        //    {
        //        var result = await _carRepo.GetCustomerCarsAsync(search, brand, color, minPrice, maxPrice, page, pageSize);

        //        // BÍ KÍP Ở ĐÂY: Chỉ lấy đúng những thông tin FE cần để vẽ lên giao diện
        //        var cleanCars = result.Cars.Select(c => new
        //        {
        //            c.CarId,
        //            c.Name,
        //            c.Brand,
        //            c.Price,
        //            c.ImageUrl,
        //            Status = c.Status.ToString() // Ép cái số 1, 2, 3 thành chữ cho dễ đọc
        //        });

        //        return Ok(new
        //        {
        //            TotalItems = result.TotalCount,
        //            CurrentPage = page,
        //            PageSize = pageSize,
        //            TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize),
        //            Data = cleanCars // Đưa mảng dữ liệu đã làm sạch vào đây!
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
        //    }
        //}

        // GET: api/Cars
        [HttpGet]
        public async Task<IActionResult> GetCarsForCustomer(
            [FromQuery] string? search,
            [FromQuery] string? brand,
            [FromQuery] string? color,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] CarStatus? status, // 👈 THÊM DÒNG NÀY ĐỂ HỨNG YÊU CẦU LỌC TỪ FE
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12)
        {
            try
            {
                // 👈 NHỚ TRUYỀN THÊM 'status' VÀO CHO THỦ KHO TÌM KIẾM
                var result = await _carRepo.GetCustomerCarsAsync(search, brand, color, minPrice, maxPrice, status, page, pageSize);

                // BÍ KÍP Ở ĐÂY: Chỉ lấy đúng những thông tin FE cần để vẽ lên giao diện
                var cleanCars = result.Cars.Select(c => new
                {
                    c.CarId,
                    c.Name,
                    c.Brand,
                    c.Price,
                    c.ImageUrl,
                    Status = c.Status.ToString() // Ép cái số thành chữ cho FE dễ làm nhãn
                });

                return Ok(new
                {
                    TotalItems = result.TotalCount,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize),
                    Data = cleanCars
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
            // 1. Nhờ thủ kho lấy xe + toàn bộ ảnh lên
            var car = await _carRepo.GetCarDetailForCustomerAsync(id);

            // 2. Chặn ngay nếu không tìm thấy (hoặc xe đang nháp/thùng rác)
            if (car == null)
            {
                return NotFound(new { message = "Chiếc xe này không tồn tại, đã bị xóa hoặc chưa được mở bán!" });
            }

            try
            {
                // 3. ĐÓNG GÓI DỮ LIỆU (DTO) SIÊU SẠCH CHO KHÁCH HÀNG (CUSTOMER)
                var carDetail = new
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
                    car.Description,
                    car.ImageUrl, // Tấm ảnh Đại diện chính
                    Status = car.Status.ToString(),

                    // BÍ KÍP: Lọc riêng Album ảnh và GOM NHÓM theo phân loại (Nội thất, Ngoại thất...)
                    GalleryImages = car.CarImages
                                       .Where(img => img.Is360Degree == false)
                                       .GroupBy(img => img.ImageType)
                                       .Select(group => new {
                                           Category = group.Key, // Tên tab (VD: "Nội thất")
                                                                 // Khách hàng CHỈ CẦN link ảnh để xem, không cần ID để xóa
                                           Images = group.Select(i => i.ImageUrl).ToList()
                                       }).ToList(),

                    // Lọc riêng bộ 360 độ cho FE nạp vào thư viện xoay
                    Images360 = car.CarImages
                                   .Where(img => img.Is360Degree == true)
                                   .Select(img => img.ImageUrl)
                                   .ToList()
                };

                return Ok(new { message = "Lấy thông tin chi tiết xe thành công!", data = carDetail });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
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
