using Microsoft.AspNetCore.Mvc;
using CoreEntities.Models;
using LogicBusiness.Interfaces.Admin;

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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarDetailForAdmin(int id)
        {
            var carDetail = await _adminService.GetCarDetailAsync(id);
            if (carDetail == null) return NotFound(new { message = "Không tìm thấy xe này trong hệ thống!" });
            return Ok(carDetail);
        }

        [HttpPost]
        public async Task<IActionResult> PostCar([FromForm] Car car)
        {
            var result = await _adminService.CreateCarAsync(car, car.ImageFile);
            if (!result.Success) return BadRequest(result.Message); // Viết hoa chữ S và M
            return Ok(result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCar(int id, [FromForm] Car car)
        {
            if (id != car.CarId) return BadRequest("Dữ liệu ID không đồng nhất.");
            var result = await _adminService.UpdateCarAsync(id, car);
            if (!result.Success) return NotFound(result.Message);
            return Ok(new { message = result.Message, car = result.Data });
        }

        [HttpPost("{carId}/images")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadCarImage([FromRoute] int carId, IFormFile file, [FromForm] string imageType)
        {
            if (file == null || file.Length == 0) return BadRequest("Vui lòng chọn một file ảnh!");
            var result = await _adminService.UploadGalleryImageAsync(carId, file, imageType);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(new { message = $"Thêm ảnh phụ thành công!", data = result.Data });
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