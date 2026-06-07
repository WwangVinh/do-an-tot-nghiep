using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OtoBackend.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/admin/consult-requests")]
    public class ConsultRequestAdminController : ControllerBase
    {
        private readonly IConsultRequestAdminService _service;

        public ConsultRequestAdminController(IConsultRequestAdminService service)
        {
            _service = service;
        }

        // ============================================================
        // Helper: lấy thông tin user từ JWT claims
        // CHỈNH LẠI tên claim cho khớp với JWT của cậu nếu cần.
        // Nếu codebase đã có BaseController hoặc helper sẵn cho việc này
        // thì thay thế các method dưới đây.
        // ============================================================
        private int GetCurrentUserId()
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("userId")?.Value
                     ?? User.FindFirst("sub")?.Value;
            return int.TryParse(idStr, out var id) ? id : 0;
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value
                ?? User.FindFirst("role")?.Value
                ?? string.Empty;
        }

        private int? GetCurrentShowroomId()
        {
            var s = User.FindFirst("ShowroomId")?.Value;
            return int.TryParse(s, out var id) ? id : (int?)null;
        }

        // ============================================================
        // GET: api/admin/consult-requests?page=1&pageSize=10&status=Pending&requestType=Quotation&search=
        // Danh sách yêu cầu (phân trang + filter)
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] string? requestType = null)
        {
            var result = await _service.GetListAsync(
                page, pageSize, search, status, requestType,
                GetCurrentUserRole(), GetCurrentShowroomId(), GetCurrentUserId());

            return Ok(new { success = true, data = result });
        }

        // ============================================================
        // GET: api/admin/consult-requests/{id}
        // Chi tiết 1 yêu cầu
        // ============================================================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var detail = await _service.GetDetailAsync(id, GetCurrentUserRole(), GetCurrentShowroomId());
            if (detail == null)
                return NotFound(new { success = false, message = "Không tìm thấy yêu cầu hoặc bạn không có quyền xem." });

            return Ok(new { success = true, data = detail });
        }

        // ============================================================
        // PUT: api/admin/consult-requests/{id}/pickup
        // Sales bấm "Tiếp nhận tư vấn"
        // ============================================================
        [HttpPut("{id:int}/pickup")]
        public async Task<IActionResult> Pickup(int id, [FromBody] ConsultPickupDto dto)
        {
            var (success, message) = await _service.PickupAsync(
                id, dto ?? new ConsultPickupDto(),
                GetCurrentUserId(), GetCurrentUserRole(), GetCurrentShowroomId());

            if (!success)
                return BadRequest(new { success, message });

            return Ok(new { success, message });
        }

        // ============================================================
        // PUT: api/admin/consult-requests/{id}/result
        // Sales chốt kết quả: Thành công (IsSuccess=true) hoặc Thất bại (false)
        // ============================================================
        [HttpPut("{id:int}/result")]
        public async Task<IActionResult> SubmitResult(int id, [FromBody] ConsultResultDto dto)
        {
            if (dto == null)
                return BadRequest(new { success = false, message = "Vui lòng nhập kết quả tư vấn." });

            var (success, message) = await _service.SubmitResultAsync(
                id, dto,
                GetCurrentUserId(), GetCurrentUserRole(), GetCurrentShowroomId());

            if (!success)
                return BadRequest(new { success, message });

            return Ok(new { success, message });
        }

        // ============================================================
        // PUT: api/admin/consult-requests/{id}/cancel
        // Admin/Manager/Sales hủy yêu cầu (kèm lý do)
        // ============================================================
        [HttpPut("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id, [FromBody] BookingCancelDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.CancelReason))
                return BadRequest(new { success = false, message = "Vui lòng nhập lý do hủy." });

            var (success, message) = await _service.CancelByAdminAsync(
                id, dto.CancelReason,
                GetCurrentUserId(), GetCurrentUserRole(), GetCurrentShowroomId());

            if (!success)
                return BadRequest(new { success, message });

            return Ok(new { success, message });
        }

        // ============================================================
        // GET: api/admin/consult-requests/stats
        // Thống kê số lượng theo từng trạng thái (cho dashboard)
        // ============================================================
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _service.GetStatsAsync(GetCurrentUserRole(), GetCurrentShowroomId());
            return Ok(new { success = true, data = stats });
        }
    }
}