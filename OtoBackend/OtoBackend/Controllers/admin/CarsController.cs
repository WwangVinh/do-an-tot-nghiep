using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace OtoBackend.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly ICarAdminService _adminService;

        public CarsController(ICarAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCarsForAdmin([FromQuery] string? search, [FromQuery] CarStatus? status, [FromQuery] bool? isDeleted = false, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _adminService.GetCarsAsync(search, status, isDeleted, page, pageSize));
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAdminCars([FromQuery] CarFilterDto filter)
        //{
        //    // Gọi Service -> Gọi Repo (truyền isAdmin = true)
        //    var cars = await _adminService.GetCarsAsync(filter, isAdmin: true);

        //    return Ok(new { success = true, data = cars });
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarDetailForAdmin(int id)
        {
            var carDetail = await _adminService.GetCarDetailAsync(id);
            if (carDetail == null) return NotFound(new { message = "Không tìm thấy xe này trong hệ thống!" });
            return Ok(carDetail);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCar([FromForm] CarCreateDto dto)
        {
            // Đổi _carAdminService thành _adminService cho khớp với code cũ của ní
            var result = await _adminService.CreateCarAsync(dto);

            if (result.Success)
            {
                return Ok(new { message = result.Message, data = result.Data });
            }

            return BadRequest(new { message = result.Message });
        }

        //[HttpPost]
        //public async Task<IActionResult> PostCar([FromForm] Car car)
        //{
        //    var result = await _adminService.CreateCarAsync(car, car.ImageFile);
        //    if (!result.Success) return BadRequest(result.Message); // Viết hoa chữ S và M
        //    return Ok(result.Data);
        //}

        //[HttpGet("suggest-spec-names")]
        //public async Task<IActionResult> GetSuggestedSpecNames()
        //{
        //    // Lấy danh sách các tên thông số không trùng nhau (Distinct) từ bảng có sẵn
        //    var suggestions = await _context.CarSpecifications
        //                                    .Select(s => s.SpecName)
        //                                    .Distinct()
        //                                    .ToListAsync();
        //    return Ok(suggestions);
        //}

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")] // Quan trọng để nhận được ảnh và chuỗi Specifications
        public async Task<IActionResult> PutCar(int id, [FromForm] CarUpdateDto dto)
        {
            // 1. Kiểm tra tính hợp lệ của dữ liệu đầu vào (Validation)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 2. Gọi Service thực hiện "Xóa cũ - Xây mới"
            var result = await _adminService.UpdateCarAsync(id, dto);

            // 3. Trả về kết quả cho Frontend (React)
            if (result.Success)
            {
                return Ok(new
                {
                    message = result.Message,
                    data = result.Car
                });
            }

            return BadRequest(new { message = result.Message });
        }

        [HttpPost("{carId}/images")]
        [Consumes("multipart/form-data")]
        // 👈 Đổi IFormFile file thành List<IFormFile> files
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
            return Ok(new { message = $"Đã tải lên {files.Count} ảnh 360 thành công!", data = result.Data });
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
            int currentAdminId = 1; // Tạm thời Fake, sau này lấy từ Token
            var success = await _adminService.SoftDeleteCarAsync(id, currentAdminId);
            if (!success) return BadRequest("Xe này không tồn tại hoặc đã nằm trong thùng rác!");
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
        public async Task<IActionResult> HardDeleteCar(int id)
        {
            var success = await _adminService.HardDeleteCarAsync(id);
            if (!success) return BadRequest("Xe này không tồn tại!");
            return Ok(new { message = "Đã tiêu diệt chiếc xe và dọn sạch ổ cứng vĩnh viễn khỏi vũ trụ!" });
        }
    }
}