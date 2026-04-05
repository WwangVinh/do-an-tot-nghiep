using CoreEntities.Models.DTOs;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt; // Nhớ có dòng này nha
using System.Security.Claims;

namespace OtoBackend.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    // 👇 Nới lỏng cửa cho cả Manager vào, Service sẽ tự phân xử bên trong
    [Authorize(Roles = "Admin, ShowroomManager")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // 1. LẤY DANH SÁCH USER
        [HttpGet]
        public async Task<IActionResult> GetAllUsers(
        [FromQuery] string userType = "Staff", // Mặc định lấy Staff (truyền Customer nếu muốn lấy khách)
        [FromQuery] bool isDeleted = false,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] int? filterShowroomId = null)
        {
            // ✂️ BÓC TOKEN LẤY THẺ NGÀNH
            string currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            string? claimShowroomId = User.FindFirst("ShowroomId")?.Value;
            int? currentUserShowroomId = string.IsNullOrEmpty(claimShowroomId) ? null : int.Parse(claimShowroomId);

            // Chuyền bóng xuống Service
            var result = await _userService.GetFilteredUsersAsync(userType, isDeleted, search, page, pageSize, currentUserRole, currentUserShowroomId, filterShowroomId);
            return Ok(result);
        }

        // 2. KHÓA / XÓA USER (Đổi thành HttpPut vì mình dùng hàm Handle chung)
        [HttpPut("{id}/status")]
        public async Task<IActionResult> HandleUserStatus(int id, [FromQuery] string action) // Truyền action = "Delete", "Deactivate", hoặc "Activate"
        {
            // ✂️ BÓC TOKEN (Lấy thêm ID của người đang bấm nút)
            string? claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            int currentUserId = string.IsNullOrEmpty(claimId) ? 0 : int.Parse(claimId);

            string currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            string? claimShowroomId = User.FindFirst("ShowroomId")?.Value;
            int? currentUserShowroomId = string.IsNullOrEmpty(claimShowroomId) ? null : int.Parse(claimShowroomId);

            var result = await _userService.HandleUserStatusAsync(id, action, currentUserId, currentUserRole, currentUserShowroomId);

            if (!result.Success) return BadRequest(new { Message = result.Message });
            return Ok(new { Message = result.Message });
        }

        // 3. TẠO TÀI KHOẢN NHÂN VIÊN
        [HttpPost("staff")]
        public async Task<IActionResult> CreateStaffAccount([FromBody] StaffAccountRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // ✂️ BÓC TOKEN LẤY THẺ NGÀNH
            string currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            string? claimShowroomId = User.FindFirst("ShowroomId")?.Value;
            int? currentUserShowroomId = string.IsNullOrEmpty(claimShowroomId) ? null : int.Parse(claimShowroomId);

            var result = await _userService.CreateStaffAccountAsync(request, currentUserRole, currentUserShowroomId);

            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }
    }
}