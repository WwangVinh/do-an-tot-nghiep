using LogicBusiness.Interfaces.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OtoBackend.Controllers.Customer
{
    [Route("api/customer/[controller]")]
    [ApiController]
    [Authorize] // Bắt buộc đăng nhập mới được xài tim
    public class WishlistsController : ControllerBase
    {
        private readonly ICarWishlistService _wishlistService;

        public WishlistsController(ICarWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        // BẤM VÀO ICON TRÁI TIM Ở FE SẼ GỌI API NÀY
        [HttpPost("toggle/{carId}")]
        public async Task<IActionResult> ToggleWishlist(int carId)
        {
            var userIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                         ?? User.Claims.FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Unauthorized();

            var result = await _wishlistService.ToggleWishlistAsync(userId, carId);

            if (!result.Success) return BadRequest(new { Success = false, result.Message });

            // FE nhận cục này sẽ biết đường tô đỏ hay làm rỗng hình trái tim
            return Ok(new { Success = true, result.Message, result.IsHearted });
        }

        // VÀO TRANG CÁ NHÂN ĐỂ XEM DANH SÁCH XE ĐÃ LƯU
        [HttpGet("my-wishlist")]
        public async Task<IActionResult> GetMyWishlist()
        {
            var userIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                         ?? User.Claims.FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Unauthorized();

            var data = await _wishlistService.GetMyWishlistAsync(userId);
            return Ok(new { Success = true, Data = data });
        }
    }
}