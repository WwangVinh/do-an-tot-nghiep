using Microsoft.AspNetCore.Mvc;
using CoreEntities.Models;
using LogicBusiness.Interfaces.Customer;

namespace OtoBackend.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarsController(ICarService carService)
        {
            _carService = carService;
        }

        // GET: api/Cars
        [HttpGet]
        public async Task<IActionResult> GetCustomerCars(
        [FromQuery] string? search, [FromQuery] string? brand, [FromQuery] string? color,
        [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] CarStatus? status,
        [FromQuery] string? transmission, [FromQuery] string? bodyStyle,
        [FromQuery] string? fuelType, [FromQuery] string? location, // 👈 2 tham số mới
        [FromQuery] CarCondition? condition, [FromQuery] int? minYear, [FromQuery] int? maxYear,
        [FromQuery] string? sort, [FromQuery] bool inStockOnly = false,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _carService.GetCarsAsync(
                search, brand, color,
                minPrice, maxPrice, status,
                transmission, bodyStyle,
                fuelType, location,
                condition, minYear, maxYear,
                sort, inStockOnly,
                page, pageSize);
            return Ok(result);
        }

        // GET: api/Cars/5
        // Add int constraint so "/latest" won't be treated as id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCarDetailForCustomer(int id)
        {
            try
            {
                var carDetail = await _carService.GetCarDetailAsync(id);

                if (carDetail == null)
                {
                    return NotFound(new { message = "Chiếc xe này không tồn tại, đã bị xóa hoặc chưa được mở bán!" });
                }

                return Ok(new { message = "Lấy thông tin chi tiết xe thành công!", data = carDetail });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        // GET: api/Cars/latest?limit=6
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestCars([FromQuery] int limit = 6)
        {
            var data = await _carService.GetLatestCarsAsync(limit);
            return Ok(new { message = "Lấy danh sách xe mới nhất thành công!", data });
        }

        // GET: api/Cars/best-sellers?limit=6
        [HttpGet("best-sellers")]
        public async Task<IActionResult> GetBestSellers([FromQuery] int limit = 6)
        {
            var data = await _carService.GetBestSellingCarsAsync(limit);
            return Ok(new { message = "Lấy danh sách xe bán chạy nhất thành công!", data });
        }
    }
}