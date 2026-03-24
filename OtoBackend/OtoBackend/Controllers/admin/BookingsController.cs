using CoreEntities.Models;
using LogicBusiness.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OtoBackend.Controllers.admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin, ShowroomManager, ShowroomSales")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingAdminService _adminService;

        public BookingsController(IBookingAdminService adminService) => _adminService = adminService;

        [HttpGet("list")]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? search = null, string? status = null)
        {
            // Lấy thông tin từ Token để biết ông này là Admin hay Staff chi nhánh nào
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var showroomIdClaim = User.FindFirst("ShowroomId")?.Value;
            int? showroomId = showroomIdClaim != null ? int.Parse(showroomIdClaim) : null;

            var result = await _adminService.GetBookingsForAdminAsync(page, pageSize, search, status, role ?? "Staff", showroomId);
            return Ok(result);
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
    }
}
