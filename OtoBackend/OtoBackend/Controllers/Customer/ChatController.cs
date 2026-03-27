using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OtoBackend.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        // API này Frontend sẽ gọi lúc vừa mở khung chat lên
        [HttpGet("{sessionId}/history")]
        [AllowAnonymous] // Mở cho cả khách vãng lai
        public async Task<IActionResult> GetChatHistory(int sessionId)
        {
            var history = await _chatService.GetChatHistoryAsync(sessionId);
            return Ok(history);
        }

        [HttpGet("sessions/current")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCurrentSession([FromQuery] string? guestToken)
        {
            // Bóc tách ID người dùng từ JWT Token (nếu có đăng nhập)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int? userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : null;

            var session = await _chatService.GetCurrentSessionAsync(userId, guestToken);

            // RẤT QUAN TRỌNG: Phải trả về 404 để React biết đường gọi API tạo mới bên dưới
            if (session == null)
            {
                return NotFound(new { message = "Không có phiên chat nào đang mở." });
            }

            return Ok(session);
        }

        // Giao diện React sẽ gọi API này đầu tiên khi khách bấm mở khung chat
        [HttpGet("staff")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableStaff()
        {
            var staff = await _chatService.GetAvailableStaffAsync();
            return Ok(staff);
        }

        // 2. API: Tạo phòng chat mới
        // POST: /api/chat/sessions
        [HttpPost("sessions")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateSession([FromBody] CreateSessionRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int? userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : null;

            // Truyền thêm request.AssignedTo xuống Service
            var newSession = await _chatService.CreateSessionAsync(userId, request.GuestToken, request.AssignedTo);

            return Ok(newSession);
        }
    }
}
