using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OtoBackend.Controllers.admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Mặc định khóa toàn bộ, chỉ Admin được vào
    public class ShowroomsController : ControllerBase
    {
        private readonly IShowroomService _showroomService;

        public ShowroomsController(IShowroomService showroomService)
        {
            _showroomService = showroomService;
        }

        // GET: api/admin/showrooms (Mở khóa cho Khách hàng xem danh sách)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllShowrooms()
        {
            var showrooms = await _showroomService.GetAllShowroomsAsync();
            return Ok(showrooms);
        }

        // GET: api/admin/showrooms/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetShowroomById(int id)
        {
            var showroom = await _showroomService.GetShowroomByIdAsync(id);
            if (showroom == null) return NotFound(new { message = "Không tìm thấy Showroom này!" });
            return Ok(showroom);
        }

        // POST: api/admin/showrooms (Chỉ Admin)
        [HttpPost]
        public async Task<IActionResult> CreateShowroom([FromBody] ShowroomRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _showroomService.CreateShowroomAsync(request);
            return CreatedAtAction(nameof(GetShowroomById), new { id = result.ShowroomId }, new { message = "Thêm mới Showroom thành công!", data = result });
        }

        // PUT: api/admin/showrooms/5 (Chỉ Admin)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShowroom(int id, [FromBody] ShowroomRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = await _showroomService.UpdateShowroomAsync(id, request);
            if (!success) return NotFound(new { message = "Không tìm thấy Showroom để cập nhật!" });

            return Ok(new { message = "Cập nhật Showroom thành công!" });
        }

        // DELETE: api/admin/showrooms/5 (Chỉ Admin)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShowroom(int id)
        {
            var success = await _showroomService.DeleteShowroomAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy Showroom để xóa!" });

            return Ok(new { message = "Xóa Showroom thành công!" });
        }

        [HttpGet("{id}/cars")]
        [AllowAnonymous] // Phải có cờ này để Khách hàng vãng lai xem được xe trên Web
        public async Task<IActionResult> GetCarsInShowroom(int id)
        {
            // 1. Kiểm tra xem Showroom có tồn tại không
            var showroom = await _showroomService.GetShowroomByIdAsync(id);
            if (showroom == null)
            {
                return NotFound(new { message = "Không tìm thấy Showroom này!" });
            }

            // 2. Lấy danh sách xe trong kho của Showroom đó
            var cars = await _showroomService.GetCarsInShowroomAsync(id);

            // 3. Trả về Frontend 1 cục JSON xịn xò chứa cả info Showroom và info Xe
            return Ok(new
            {
                message = "Lấy dữ liệu thành công!",
                showroomInfo = showroom,
                inventory = cars
            });
        }
    }
}
