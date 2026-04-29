using LogicBusiness.Interfaces.Admin;
using LogicBusiness.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OtoBackend.Controllers.Admin
{
    [Route("api/admin/bookings")]
    [ApiController]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales},{AppRoles.Technician}")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingAdminService _service;

        public BookingsController(IBookingAdminService service) => _service = service;

        private (string role, int? showroomId) GetClaims()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "Staff";
            var raw = User.FindFirst("ShowroomId")?.Value;
            int? showroomId = int.TryParse(raw, out var sid) ? sid : null;
            return (role, showroomId);
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? search = null, string? status = null)
        {
            var (role, showroomId) = GetClaims();
            return Ok(await _service.GetBookingsForAdminAsync(page, pageSize, search, status, role, showroomId));
        }

        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var (role, showroomId) = GetClaims();
            var detail = await _service.GetBookingDetailAsync(id, role, showroomId);
            if (detail == null) return NotFound(new { message = "Không tìm thấy lịch hẹn hoặc bạn không có quyền truy cập." });
            return Ok(detail);
        }

        [HttpPost("{id}/consult")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales}")]
        public async Task<IActionResult> Consult(int id, [FromBody] BookingConsultDto dto)
        {
            var (role, showroomId) = GetClaims();
            var result = await _service.MarkAsConsultedAsync(id, dto, role, showroomId);
            return result.Success ? Ok(new { message = result.Message }) : BadRequest(new { message = result.Message });
        }

        [HttpPost("{id}/send-tech")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales}")]
        public async Task<IActionResult> SendToTech(int id, [FromBody] BookingSendTechDto dto)
        {
            var (role, showroomId) = GetClaims();
            var result = await _service.SendToTechCheckAsync(id, dto, role, showroomId);
            return result.Success ? Ok(new { message = result.Message }) : BadRequest(new { message = result.Message });
        }

        [HttpPost("{id}/tech-result")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Technician}")]
        public async Task<IActionResult> TechResult(int id, [FromBody] BookingTechResultDto dto)
        {
            var (role, showroomId) = GetClaims();
            var result = await _service.SubmitTechResultAsync(id, dto, role, showroomId);
            return result.Success ? Ok(new { message = result.Message }) : BadRequest(new { message = result.Message });
        }

        [HttpPost("{id}/confirm")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales}")]
        public async Task<IActionResult> Confirm(int id)
        {
            var (role, showroomId) = GetClaims();
            var result = await _service.ConfirmBookingAsync(id, role, showroomId);
            return result.Success ? Ok(new { message = result.Message }) : BadRequest(new { message = result.Message });
        }

        [HttpPost("{id}/complete")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales}")]
        public async Task<IActionResult> Complete(int id, [FromBody] BookingCompleteDto? dto)
        {
            var (role, showroomId) = GetClaims();
            var result = await _service.CompleteBookingAsync(id, dto?.ResultNote, role, showroomId);
            return result.Success ? Ok(new { message = result.Message }) : BadRequest(new { message = result.Message });
        }

        [HttpPost("{id}/no-show")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales}")]
        public async Task<IActionResult> NoShow(int id, [FromBody] BookingNoShowDto dto)
        {
            var (role, showroomId) = GetClaims();
            var result = await _service.MarkNoShowAsync(id, dto, role, showroomId);
            return result.Success ? Ok(new { message = result.Message }) : BadRequest(new { message = result.Message });
        }

        [HttpPost("{id}/cancel")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales}")]
        public async Task<IActionResult> Cancel(int id, [FromBody] BookingCancelDto dto)
        {
            var (role, showroomId) = GetClaims();
            var result = await _service.CancelBookingByAdminAsync(id, dto, role, showroomId);
            return result.Success ? Ok(new { message = result.Message }) : BadRequest(new { message = result.Message });
        }

        [HttpGet("pending-tech")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager},{AppRoles.Technician}")]
        public async Task<IActionResult> GetPendingTech()
        {
            var (role, showroomId) = GetClaims();
            return Ok(await _service.GetPendingTechCheckAsync(role, showroomId));
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var (role, showroomId) = GetClaims();
            return Ok(await _service.GetBookingStatsAsync(role, showroomId));
        }
    }
}