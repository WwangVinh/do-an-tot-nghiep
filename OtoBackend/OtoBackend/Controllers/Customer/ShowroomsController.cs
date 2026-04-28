using LogicBusiness.Interfaces.Admin;
using Microsoft.AspNetCore.Mvc;

namespace OtoBackend.Controllers.Customer
{
    [Route("api/showrooms")] // Route sạch sẽ cho khách: api/showrooms
    [ApiController]
    public class ShowroomsController : ControllerBase
    {
        private readonly IShowroomService _showroomService;

        // 👇 Vẫn inject cái Service cũ vào đây, không cần viết lại Service mới!
        public ShowroomsController(IShowroomService showroomService)
        {
            _showroomService = showroomService;
        }

        // Khách xem tất cả showroom
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _showroomService.GetAllShowroomsAsync();
            return Ok(result);
        }

        // Khách xem chi tiết 1 showroom
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _showroomService.GetShowroomByIdAsync(id);
            if (result == null) return NotFound(new { Message = "Không tìm thấy cơ sở" });
            return Ok(result);
        }

        // Khách xem xe tại showroom đó
        [HttpGet("{id}/cars")]
        public async Task<IActionResult> GetCarsInShowroom(int id)
        {
            var cars = await _showroomService.GetCarsInShowroomAsync(id);
            return Ok(cars);
        }
    }
}