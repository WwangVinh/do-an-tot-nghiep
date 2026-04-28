using LogicBusiness.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LogicBusiness.Utilities;
using System.IO;
using LogicBusiness.Interfaces.Admin;
using System.Security.Claims; // 👈 Thêm cái này để móc túi lấy ID người dùng

namespace OtoBackend.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    // 👇 Đã mở cửa cho Marketing và Content vào làm việc
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales},{AppRoles.Marketing},{AppRoles.Content}")]
    public class BannersController : ControllerBase
    {
        private readonly IBannerAdminService _service;

        public BannersController(IBannerAdminService service)
        {
            _service = service;
        }

        // --- Hàm lấy ID người đang đăng nhập (viết gọn dùng chung) ---
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out int id) ? id : 0;
        }

        [HttpPost("upload-image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage([FromForm] BannerUploadRequest request)
        {
            if (request.File == null || request.File.Length == 0) return BadRequest(new { message = "Vui lòng chọn file ảnh." });

            var ext = Path.GetExtension(request.File.FileName)?.ToLowerInvariant();
            var allowed = new HashSet<string> { ".jpg", ".jpeg", ".png", ".webp" };
            if (string.IsNullOrEmpty(ext) || !allowed.Contains(ext))
            {
                return BadRequest(new { message = "Chỉ hỗ trợ ảnh .jpg, .jpeg, .png, .webp" });
            }

            var target = string.IsNullOrWhiteSpace(request.FolderName) ? "banner" : request.FolderName;
            var imageUrl = await FileHelper.UploadFileAsync(request.File, "Banners", target, customFileName: $"banner-{Guid.NewGuid():N}");
            return Ok(new { message = "Upload ảnh banner thành công!", imageUrl });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? isActive = null)
        {
            var data = await _service.GetAllAsync(isActive);
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var banner = await _service.GetByIdAsync(id);
            if (banner == null) return NotFound(new { message = "Không tìm thấy banner!" });
            return Ok(banner);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BannerCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int userId = GetCurrentUserId(); // 👈 Lấy ID người làm

            var result = await _service.CreateAsync(dto, userId); // 👈 Truyền xuống Service để bắn thông báo
            if (!result.Success) return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message, data = result.Data });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BannerUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int userId = GetCurrentUserId(); // 👈 Lấy ID

            var result = await _service.UpdateAsync(id, dto, userId); // 👈 Truyền xuống Service
            if (!result.Success) return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message, data = result.Data });
        }

        [HttpDelete("{id}")]
        // 👇 Nếu ní muốn cấm Marketing xóa banner (chỉ cho Sếp xóa), thì thêm [Authorize] riêng ở đây:
        // [Authorize(Roles = "Admin, ShowroomManager, SalesManager")]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = GetCurrentUserId(); // 👈 Lấy ID

            var result = await _service.DeleteAsync(id, userId); // 👈 Truyền xuống Service

            if (!result.Success)
            {
                return NotFound(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }
    }
}