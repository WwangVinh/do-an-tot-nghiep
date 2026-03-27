using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OtoBackend.Controllers.admin
{
    [Route("api/admin/showrooms")] // Đã đổi thành số nhiều /showrooms
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ShowroomsController : ControllerBase // Đã thêm 's' vào tên Class
    {
        private readonly IShowroomService _showroomService;

        public ShowroomsController(IShowroomService showroomService)
        {
            _showroomService = showroomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _showroomService.GetAllShowroomsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _showroomService.GetShowroomByIdAsync(id);
            if (result == null) return NotFound(new { Message = "Không tìm thấy cơ sở" });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ShowroomCreateDto dto)
        {
            var result = await _showroomService.CreateShowroomAsync(dto);
            if (!result.Success) return BadRequest(new { result.Message });
            return Ok(new { result.Message });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ShowroomUpdateDto dto)
        {
            var result = await _showroomService.UpdateShowroomAsync(id, dto);
            if (!result.Success) return BadRequest(new { result.Message });
            return Ok(new { result.Message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _showroomService.DeleteShowroomAsync(id);
            if (!result.Success) return BadRequest(new { result.Message });
            return Ok(new { result.Message });
        }

        // API dành cho Khách xem xe tại Showroom cụ thể
        [HttpGet("{id}/cars")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCarsInShowroom(int id)
        {
            var showroom = await _showroomService.GetShowroomByIdAsync(id);
            if (showroom == null)
            {
                return NotFound(new { message = "Không tìm thấy Showroom này!" });
            }

            // Gọi hàm từ Service (Lỗi CS1061 sẽ biến mất sau khi làm Bước 1)
            var cars = await _showroomService.GetCarsInShowroomAsync(id);

            return Ok(new
            {
                message = "Lấy dữ liệu thành công!",
                showroomInfo = showroom,
                inventory = cars
            });
        }
    }
}