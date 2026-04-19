using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Customer;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace OtoBackend.Controllers.admin
{
    [Route("api/admin/banners")]
    [ApiController]
    [Authorize(Roles = "Admin, ShowroomManager")] // Sales không cần vào chỉnh sửa Banner
    public class AdminBannersController : ControllerBase
    {
        private readonly IBannerService _bannerService;

        public AdminBannersController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        // GET: api/admin/banners
        [HttpGet]
        public async Task<IActionResult> GetAllBanners()
        {
            var banners = await _bannerService.GetAllAdminBannersAsync();
            return Ok(banners);
        }

        // POST: api/admin/banners
        [HttpPost]
        public async Task<IActionResult> CreateBanner([FromBody] BannerCreateUpdateDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var newBanner = await _bannerService.CreateBannerAsync(request);
            return Ok(newBanner);
        }

        // PATCH: api/admin/banners/5/toggle-status
        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var success = await _bannerService.ToggleBannerStatusAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy Banner!" });

            return Ok(new { message = "Cập nhật trạng thái thành công!" });
        }
    }
}
