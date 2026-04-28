using CoreEntities.Models;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.DTOs; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OtoBackend.Controllers.admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales}")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingAdminService _adminService;

        public BookingsController(IBookingAdminService adminService) => _adminService = adminService;

        [HttpGet("list")]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? search = null, string? status = null)
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var showroomIdClaim = User.FindFirst("ShowroomId")?.Value;
            int? showroomId = showroomIdClaim != null ? int.Parse(showroomIdClaim) : null;

            var result = await _adminService.GetBookingsForAdminAsync(page, pageSize, search, status, role ?? "Staff", showroomId);
            return Ok(result);
        }

        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var showroomIdClaim = User.FindFirst("ShowroomId")?.Value;
            int? showroomId = showroomIdClaim != null ? int.Parse(showroomIdClaim) : null;

            var detail = await _adminService.GetBookingDetailAsync(id, role ?? "Staff", showroomId);
            if (detail == null) return NotFound(new { message = "Không tìm thấy lịch hẹn hoặc không có quyền truy cập." });
            return Ok(detail);
        }

        [HttpPut("update-status/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string newStatus)
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var showroomIdClaim = User.FindFirst("ShowroomId")?.Value;
            int? showroomId = showroomIdClaim != null ? int.Parse(showroomIdClaim) : null;

            var result = await _adminService.UpdateBookingStatusAsync(id, newStatus, role, showroomId);
            if (result.Success) return Ok(new { message = result.Message });
            return BadRequest(new { message = result.Message });
        }

        public class UpdateBookingAdminDto
        {
            public string? Status { get; set; }
            public string? Result { get; set; }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBookingAdminDto dto)
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var showroomIdClaim = User.FindFirst("ShowroomId")?.Value;
            int? showroomId = showroomIdClaim != null ? int.Parse(showroomIdClaim) : null;

            var result = await _adminService.UpdateBookingAsync(id, dto?.Status, dto?.Result, role, showroomId);
            if (result.Success) return Ok(new { message = result.Message });
            return BadRequest(new { message = result.Message });
        }
    }
}