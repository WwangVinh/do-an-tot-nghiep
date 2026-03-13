using LogicBusiness.Interfaces.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OtoBackend.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Bắt buộc phải là Admin đăng nhập mới được gọi API này
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/admin/users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers(
        [FromQuery] bool isDeleted = false,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
        {
            // Truyền tham số xuống Service
            var result = await _userService.GetFilteredUsersAsync(isDeleted, search, page, pageSize);
            return Ok(result);
        }

        // DELETE: api/admin/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // Lấy ID của Admin đang thao tác từ Token (để lưu vào DeletedBy)
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int? adminId = adminIdClaim != null ? int.Parse(adminIdClaim.Value) : null;

            var result = await _userService.DeleteUserAsync(id, adminId);

            if (!result)
            {
                return NotFound(new { Message = "Không tìm thấy người dùng hoặc người dùng đã bị xóa!" });
            }

            return Ok(new { Message = "Xóa người dùng thành công!" });
        }
    }
}