using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OtoBackend.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager},{AppRoles.ShowroomSales},{AppRoles.Sales},{AppRoles.Technician}")]
    public class CarsController : ControllerBase
    {
        private readonly ICarAdminService _adminService;

        public CarsController(ICarAdminService adminService)
        {
            _adminService = adminService;
        }

        private (string role, int? showroomId, int? userId) GetClaims()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            var showroomIdStr = User.FindFirst("ShowroomId")?.Value;
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            int? showroomId = string.IsNullOrEmpty(showroomIdStr) ? null : int.Parse(showroomIdStr);
            int? userId = string.IsNullOrEmpty(userIdStr) ? null : int.Parse(userIdStr);
            return (role, showroomId, userId);
        }

        [HttpGet]
        public async Task<IActionResult> GetCarsForAdmin(
            [FromQuery] string? search, [FromQuery] string? brand, [FromQuery] string? color,
            [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] CarStatus? status,
            [FromQuery] string? transmission, [FromQuery] string? bodyStyle, [FromQuery] string? fuelType,
            [FromQuery] string? location, [FromQuery] bool? isDeleted,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (role, showroomId, userId) = GetClaims();
            var result = await _adminService.GetCarsAsync(
                search, brand, color, minPrice, maxPrice, status,
                transmission, bodyStyle, fuelType, location, isDeleted, page, pageSize,
                showroomId, role, userId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarDetailForAdmin(int id)
        {
            var (role, showroomId, _) = GetClaims();
            var carDetail = await _adminService.GetCarDetailAsync(id, role, showroomId);
            if (carDetail == null) return NotFound(new { message = "Không tìm thấy xe này trong hệ thống!" });
            return Ok(carDetail);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCar([FromForm] CarCreateDto dto)
        {
            var (role, showroomId, userId) = GetClaims();
            var result = await _adminService.CreateCarAsync(dto, role, showroomId, userId);
            if (result.Success) return Ok(new { message = result.Message, data = result.Data });
            return BadRequest(new { message = result.Message });
        }

        [HttpPost("full")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateCarFull([FromForm] CarCreateFullDto dto)
        {
            var (role, showroomId, userId) = GetClaims();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _adminService.CreateCarFullAsync(dto, role, showroomId, userId);
            if (result.Success) return Ok(new { message = result.Message, data = result.Data });
            return BadRequest(new { message = result.Message });
        }

        [HttpPut("full/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateCarFull([FromRoute] int id, [FromForm] CarCreateFullDto dto)
        {
            var (role, showroomId, _) = GetClaims();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _adminService.UpdateCarFullAsync(id, dto, role, showroomId);
            if (result.Success) return Ok(new { message = result.Message, data = result.Data });
            return BadRequest(new { message = result.Message });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCar(int id, [FromForm] CarUpdateDto dto)
        {
            var (role, showroomId, _) = GetClaims();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _adminService.UpdateCarAsync(id, dto, role, showroomId);
            if (result.Success) return Ok(new { message = result.Message, data = result.Car });
            return BadRequest(new { message = result.Message });
        }

        [HttpPost("{carId}/images")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadCarImages(
            [FromRoute] int carId, [FromForm] List<IFormFile> files,
            [FromForm] List<string>? titles, [FromForm] List<string> descriptions,
            [FromForm] string imageType)
        {
            if (files == null || files.Count == 0) return BadRequest("Vui lòng chọn ít nhất một file ảnh!");
            var result = await _adminService.UploadGalleryImagesAsync(carId, files, titles, descriptions, imageType);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(new { message = result.Message, data = result.Data });
        }

        [HttpPut("images/{imageId}/details")]
        [Consumes("multipart/form-data", "application/x-www-form-urlencoded")]
        public async Task<IActionResult> UpdateImageDetails(int imageId, [FromForm] UpdateImageDetailsDto dto)
        {
            var success = await _adminService.UpdateImageDetailsAsync(imageId, dto.Title, dto.Description);
            if (!success) return BadRequest(new { message = "Không thể cập nhật! Ảnh không tồn tại hoặc đây là ảnh 360 độ." });
            return Ok(new { message = "Cập nhật tiêu đề và mô tả ảnh thành công!" });
        }

        [HttpPost("{id}/upload-360")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload360Image([FromRoute] int id, [FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0) return BadRequest("Vui lòng chọn ít nhất 1 file ảnh 360.");
            var result = await _adminService.Upload360ImagesAsync(id, files);
            if (!result.Success) return NotFound(result.Message);
            return Ok(new { message = $"Đã tải lên {files.Count} ảnh 360 thành công!" });
        }

        [HttpDelete("delete-image/{imageId}")]
        public async Task<IActionResult> DeleteCarImage(int imageId)
        {
            var success = await _adminService.DeleteCarImageAsync(imageId);
            if (!success) return NotFound("Không tìm thấy ảnh này.");
            return Ok(new { message = "Đã xóa ảnh vĩnh viễn thành công!" });
        }

        [HttpDelete("{id}/SoftDeleteCar")]
        public async Task<IActionResult> SoftDeleteCar(int id)
        {
            var (_, _, userId) = GetClaims();
            await _adminService.SoftDeleteCarAsync(id, userId ?? 0);
            return Ok(new { message = "Đã đưa xe vào thùng rác thành công!" });
        }

        [HttpPut("{id}/restore")]
        public async Task<IActionResult> RestoreCar(int id)
        {
            var success = await _adminService.RestoreCarAsync(id);
            if (!success) return BadRequest("Không tìm thấy xe hoặc xe đang bán trên sàn!");
            return Ok(new { message = "Đã hồi sinh xe thành công, sẵn sàng lên sàn!" });
        }

        [HttpDelete("{id}/hard-delete")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> HardDeleteCar(int id)
        {
            var (role, _, _) = GetClaims();
            var success = await _adminService.HardDeleteCarAsync(id, role);
            if (!success) return BadRequest(new { message = "Xe này không tồn tại hoặc ní không đủ quyền xóa!" });
            return Ok(new { message = "Đã tiêu diệt chiếc xe và dọn sạch ổ cứng vĩnh viễn khỏi vũ trụ!" });
        }

        [HttpPost("{id}/clone")]
        public async Task<IActionResult> CloneCar(int id)
        {
            var (role, showroomId, _) = GetClaims();
            var result = await _adminService.CloneCarAsync(id, role, showroomId);
            if (result.Success) return Ok(new { message = result.Message, newCarId = result.NewCarId });
            return BadRequest(new { message = result.Message });
        }

        [HttpPut("{id}/approve")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager}")]
        public async Task<IActionResult> ApproveCar(int id)
        {
            var (role, showroomId, _) = GetClaims();
            var result = await _adminService.ApproveCarAsync(id, role, showroomId);
            if (result.Success) return Ok(new { message = result.Message });
            return BadRequest(new { message = result.Message });
        }

        [HttpPut("{id}/reject")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager}")]
        public async Task<IActionResult> RejectCar(int id, [FromBody] RejectRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
                return BadRequest(new { message = "Sếp phải nhập lý do từ chối chứ!" });
            var result = await _adminService.RejectCarAsync(id, request.Reason);
            if (result.Success) return Ok(new { message = result.Message });
            return BadRequest(new { message = result.Message });
        }

        [HttpPut("{id}/change-status")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager}")]
        public async Task<IActionResult> ChangeCarStatus(int id, [FromBody] ChangeStatusRequestDto request)
        {
            var result = await _adminService.ChangeCarStatusAsync(id, request.NewStatus);
            if (result.Success) return Ok(new { message = result.Message });
            return BadRequest(new { message = result.Message });
        }
    }
}