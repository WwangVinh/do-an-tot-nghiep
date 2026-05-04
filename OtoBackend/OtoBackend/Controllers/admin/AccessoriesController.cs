using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OtoBackend.Controllers.Admin
{
    // ── Admin: CRUD phụ kiện + gán cho xe ────────────────────────────────────
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager},{AppRoles.ShowroomSales},{AppRoles.Sales},{AppRoles.Technician}")]
    public class AccessoriesController : ControllerBase
    {
        private readonly IAccessoryService _service;

        public AccessoriesController(IAccessoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound(new { message = "Không tìm thấy phụ kiện!" });
            return Ok(item);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] AccessoryCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.CreateAsync(dto);
            if (result.Success) return Ok(new { message = result.Message, data = result.Data });
            return BadRequest(new { message = result.Message });
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(int id, [FromForm] AccessoryUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.UpdateAsync(id, dto);
            if (result.Success) return Ok(new { message = result.Message });
            return BadRequest(new { message = result.Message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (result.Success) return Ok(new { message = result.Message });
            return NotFound(new { message = result.Message });
        }

        // ── Gán phụ kiện cho xe ──────────────────────────────────────────────

        /// Lấy danh sách phụ kiện đang gán cho xe
        [HttpGet("car/{carId}")]
        public async Task<IActionResult> GetByCar(int carId)
            => Ok(await _service.GetByCarIdAsync(carId));

        /// Gán thêm phụ kiện vào xe
        [HttpPost("car/{carId}")]
        public async Task<IActionResult> AssignToCar(int carId, [FromBody] AssignAccessoriesDto dto)
        {
            var result = await _service.AssignToCarAsync(carId, dto);
            if (result.Success) return Ok(new { message = result.Message });
            return BadRequest(new { message = result.Message });
        }

        /// Bỏ gán phụ kiện khỏi xe
        [HttpDelete("car/{carId}")]
        public async Task<IActionResult> RemoveFromCar(int carId, [FromBody] AssignAccessoriesDto dto)
        {
            var result = await _service.RemoveFromCarAsync(carId, dto);
            if (result.Success) return Ok(new { message = result.Message });
            return BadRequest(new { message = result.Message });
        }
    }
}
