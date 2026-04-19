using LogicBusiness.Interfaces.Admin;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace OtoBackend.Controllers.admin
{
    [Route("api/admin/files")]
    [ApiController]
    [Authorize(Roles = "Admin, ShowroomManager, ShowroomSales")] // Chỉ nội bộ mới được up ảnh
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        // POST: api/admin/files/upload
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file, [FromForm] string folder = "general")
        {
            try
            {
                // Kiểm tra định dạng đuôi file (Chỉ cho up ảnh)
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext))
                {
                    return BadRequest(new { message = "Chỉ chấp nhận file ảnh (.jpg, .png, .webp...)" });
                }

                // Kiểm tra dung lượng (Ví dụ giới hạn 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { message = "Dung lượng ảnh không được vượt quá 5MB" });
                }

                string imageUrl = await _fileService.UploadImageAsync(file, folder);

                // Trả link ảnh về cho FE
                return Ok(new { imageUrl = imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }
    }
}
