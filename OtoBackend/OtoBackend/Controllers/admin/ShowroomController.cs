using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using Microsoft.AspNetCore.Mvc;

namespace OtoBackend.Controllers.admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class ShowroomController : ControllerBase
    {
        private readonly IShowroomService _showroomService;

        public ShowroomController(IShowroomService showroomService)
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
    }
}
