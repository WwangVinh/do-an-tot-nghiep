using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet]
        public async Task<IActionResult> GetCarsForAdmin(
        [FromQuery] string? search,
        [FromQuery] string? brand,
        [FromQuery] string? color,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] CarStatus? status,
        [FromQuery] string? transmission,
        [FromQuery] string? bodyStyle,
        [FromQuery] string? fuelType,
        [FromQuery] string? location,
        [FromQuery] bool? isDeleted,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        {

            string currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            string? claimShowroomId = User.FindFirst("ShowroomId")?.Value;
            int? currentUserShowroomId = string.IsNullOrEmpty(claimShowroomId) ? null : int.Parse(claimShowroomId);
            var result = await _adminService.GetCarsAsync(
                search, brand, color, minPrice, maxPrice, status,
                transmission, bodyStyle, fuelType, location, isDeleted, page, pageSize, currentUserShowroomId);

            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarDetailForAdmin(int id)
        {

            string currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            string? claimShowroomId = User.FindFirst("ShowroomId")?.Value;
            int? currentUserShowroomId = string.IsNullOrEmpty(claimShowroomId) ? null : int.Parse(claimShowroomId);
            var carDetail = await _adminService.GetCarDetailAsync(id,currentUserRole, currentUserShowroomId);
            if (carDetail == null) return NotFound(new { message = "Không tìm thấy xe này trong hệ thống!" });
            return Ok(carDetail);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCar([FromForm] CarCreateDto dto)
        {

            string currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            string? claimShowroomId = User.FindFirst("ShowroomId")?.Value;
            int? currentUserShowroomId = string.IsNullOrEmpty(claimShowroomId) ? null : int.Parse(claimShowroomId);
            var result = await _adminService.CreateCarAsync(dto, currentUserRole, currentUserShowroomId);

            if (result.Success)
            {
                return Ok(new { message = result.Message, data = result.Data });
            }

            return BadRequest(new { message = result.Message });
        }

        // CREATE FULL: ghi đủ Cars + CarImages + CarSpecifications + CarFeatures + CarPricingVersions + CarInventories
        [HttpPost("full")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateCarFull([FromForm] CarCreateFullDto dto)
        {
            string currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            string? claimShowroomId = User.FindFirst("ShowroomId")?.Value;
            int? currentUserShowroomId = string.IsNullOrEmpty(claimShowroomId) ? null : int.Parse(claimShowroomId);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _adminService.CreateCarFullAsync(dto, currentUserRole, currentUserShowroomId);

            if (result.Success)
            {
                return Ok(new { message = result.Message, data = result.Data });
            }

            return BadRequest(new { message = result.Message });
        }

        // UPDATE FULL: cập nhật đủ Cars + Images + Specs + Features + Pricing + Inventories
        [HttpPut("full/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateCarFull([FromRoute] int id, [FromForm] CarCreateFullDto dto)
        {
            string currentUserRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "";
            string? claimShowroomId = User.FindFirst("ShowroomId")?.Value;
            int? currentUserShowroomId = string.IsNullOrEmpty(claimShowroomId) ? null : int.Parse(claimShowroomId);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _adminService.UpdateCarFullAsync(id, dto, currentUserRole, currentUserShowroomId);

            if (result.Success)
            {
                return Ok(new { message = result.Message, data = result.Data });
            }

            return BadRequest(new { message = result.Message });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutCar(int id, [FromForm] CarUpdateDto dto)
        {
            // Bóc Token lấy thông tin người dùng (Bắt buộc phải có cái này!)
            string currentUserRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "";
            string? claimShowroomId = User.FindFirst("ShowroomId")?.Value;
            int? currentUserShowroomId = string.IsNullOrEmpty(claimShowroomId) ? null : int.Parse(claimShowroomId);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Chuyền xuống Service (Thêm 2 biến vừa bóc vào cuối)
            var result = await _adminService.UpdateCarAsync(id, dto, currentUserRole, currentUserShowroomId);

            if (result.Success) return Ok(new { message = result.Message, data = result.Car });
            return BadRequest(new { message = result.Message });
        }

        [HttpPost("{carId}/images")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadCarImages(
        [FromRoute] int carId,
        [FromForm] List<IFormFile> files,
        [FromForm] List<string>? titles,
        [FromForm] List<string> descriptions,
        [FromForm] string imageType)
        {
            if (files == null || files.Count == 0)
                return BadRequest("Vui lòng chọn ít nhất một file ảnh!");

            // Gọi đúng tên hàm số nhiều mới sửa
            var result = await _adminService.UploadGalleryImagesAsync(carId, files, titles, descriptions, imageType);

            if (!result.Success)
                return BadRequest(result.Message);

            // Trả về đúng câu chữ mình vừa Build ở Service
            return Ok(new { message = result.Message, data = result.Data });
        }

        [HttpPut("images/{imageId}/details")]
        [Consumes("multipart/form-data", "application/x-www-form-urlencoded")]
        public async Task<IActionResult> UpdateImageDetails(int imageId, [FromForm] UpdateImageDetailsDto dto)
        {
            var success = await _adminService.UpdateImageDetailsAsync(imageId, dto.Title, dto.Description);

            if (!success)
                return BadRequest(new { message = "Không thể cập nhật! Ảnh không tồn tại hoặc đây là ảnh 360 độ." });

            return Ok(new { message = "Cập nhật tiêu đề và mô tả ảnh thành công!" });
        }

        [HttpPost("{id}/upload-360")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload360Image([FromRoute] int id, [FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0) return BadRequest("Vui lòng chọn ít nhất 1 file ảnh 360.");
            var result = await _adminService.Upload360ImagesAsync(id, files);
            if (!result.Success) return NotFound(result.Message);
            return Ok(new { message = $"Đã tải lên {files.Count} ảnh 360 thành công!"});
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
            string? claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            int currentUserId = string.IsNullOrEmpty(claimId) ? 0 : int.Parse(claimId);
            var success = await _adminService.SoftDeleteCarAsync(id, currentUserId);
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
            string currentUserRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "";
            var success = await _adminService.HardDeleteCarAsync(id, currentUserRole);

            if (!success) return BadRequest(new { message = "Xe này không tồn tại hoặc ní không đủ quyền xóa!" });
            return Ok(new { message = "Đã tiêu diệt chiếc xe và dọn sạch ổ cứng vĩnh viễn khỏi vũ trụ!" });
        }

        [HttpPost("{id}/clone")]
        public async Task<IActionResult> CloneCar(int id)
        {
            string currentUserRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "";
            string? claimShowroomId = User.FindFirst("ShowroomId")?.Value;
            int? currentUserShowroomId = string.IsNullOrEmpty(claimShowroomId) ? null : int.Parse(claimShowroomId);

            var result = await _adminService.CloneCarAsync(id, currentUserRole, currentUserShowroomId);

            if (result.Success)
            {
                return Ok(new { message = result.Message, newCarId = result.NewCarId });
            }

            return BadRequest(new { message = result.Message });
        }

        /// SẾP DUYỆT XE: Cho phép xe hiển thị lên Web
        [HttpPut("{id}/approve")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager}")]
        public async Task<IActionResult> ApproveCar(int id)
        {

            string currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            string? claimShowroomId = User.FindFirst("ShowroomId")?.Value;
            int? currentUserShowroomId = string.IsNullOrEmpty(claimShowroomId) ? null : int.Parse(claimShowroomId);
            var result = await _adminService.ApproveCarAsync(id, currentUserRole, currentUserShowroomId);
            if (result.Success)
            {
                return Ok(new { message = result.Message });
            }
            return BadRequest(new { message = result.Message });
        }

        /// SẾP TỪ CHỐI XE: Trả về cho nhân viên sửa lại kèm lý do
        [HttpPut("{id}/reject")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager}")]
        public async Task<IActionResult> RejectCar(int id, [FromBody] RejectRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                return BadRequest(new { message = "Sếp phải nhập lý do từ chối chứ!" });
            }  
            var result = await _adminService.RejectCarAsync(id, request.Reason);
            if (result.Success)
            {
                return Ok(new { message = result.Message });
            }
            return BadRequest(new { message = result.Message });
        }

        /// SẾP ĐỔI TRẠNG THÁI NHANH: Thích xe thành Coming Soon hay Nháp đều được
        [HttpPut("{id}/change-status")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager}")]
        public async Task<IActionResult> ChangeCarStatus(int id, [FromBody] ChangeStatusRequestDto request)
        {
            var result = await _adminService.ChangeCarStatusAsync(id, request.NewStatus);
            if (result.Success)
            {
                return Ok(new { message = result.Message });
            }
            return BadRequest(new { message = result.Message });
        }
    }
}