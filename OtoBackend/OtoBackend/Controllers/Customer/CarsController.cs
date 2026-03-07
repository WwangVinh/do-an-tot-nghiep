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
        public async Task<IActionResult> GetCarsForCustomer(
            [FromQuery] string? search,
            [FromQuery] string? brand,
            [FromQuery] string? color,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] CarStatus? status,
            [FromQuery] string? transmission, // 👈 Mở thêm ô nhập Hộp số cho Lễ tân
            [FromQuery] string? bodyStyle,    // (Đã xóa cái dấu nháy ngược vô duyên ở đây)
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12)
        {
            try
            {
                // Cất kết quả vào biến 'result'
                var result = await _carService.GetCarsAsync(search, brand, color, minPrice, maxPrice, status, transmission, bodyStyle, page, pageSize);

                // Trả đúng biến 'result' đó về cho Frontend
                return Ok(result);
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
    }
}