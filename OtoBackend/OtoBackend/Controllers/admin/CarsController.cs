using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OtoBackend.Interfaces; // Gọi Interface vào
using OtoBackend.Models;
using OtoBackend.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OtoBackend.Controllers.admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly ICarRepository _carRepo; // Dùng thủ kho

        public CarsController(ICarRepository carRepo)
        {
            _carRepo = carRepo;
        }

        // GET: api/admin/Cars
        [HttpGet]
        public async Task<IActionResult> GetCarsForAdmin(
            [FromQuery] string? search,
            [FromQuery] CarStatus? status,
            [FromQuery] bool? isDeleted = false, // Mặc định chỉ hiện xe chưa xóa
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10) // Admin mỗi trang hiện 10 dòng dạng bảng cho dễ nhìn
        {
            try
            {
                var result = await _carRepo.GetAdminCarsAsync(search, status, isDeleted, page, pageSize);

                // Admin thì trả về thông tin quản trị nhiều hơn xíu (Ngày tạo, Trạng thái xóa...)
                var adminCars = result.Cars.Select(c => new
                {
                    c.CarId,
                    c.Name,
                    c.Brand,
                    c.Price,
                    c.ImageUrl,
                    Status = c.Status.ToString(), // Hiện chữ Draft, Available cho Admin dễ đọc
                    c.IsDeleted,
                    CreatedAt = c.CreatedAt?.ToString("dd/MM/yyyy HH:mm"), // Format ngày tháng cho đẹp
                    UpdatedAt = c.UpdatedAt?.ToString("dd/MM/yyyy HH:mm")
                });

                return Ok(new
                {
                    TotalItems = result.TotalCount,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize),
                    Data = adminCars
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        // GET: api/admin/Cars/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarDetailForAdmin(int id)
        {
            // 1. Nhờ Thủ kho lấy xe lên
            var car = await _carRepo.GetCarByIdAsync(id);

            // 2. CHỈ CHẶN KHI KHÔNG TỒN TẠI TRONG DATABASE
            if (car == null)
            {
                return NotFound(new { message = "Không tìm thấy dữ liệu xe này trong hệ thống!" });
            }

            // 3. TRẢ VỀ TOÀN BỘ THÔNG TIN (Bao gồm cả "tâm linh" như ngày xóa, ngày sửa)
            return Ok(new
            {
                car.CarId,
                car.Name,
                car.Brand,
                car.Model,
                car.Year,
                car.Price,
                car.Color,
                car.Mileage,
                car.FuelType,
                car.Description,
                car.ImageUrl,

                // THÔNG TIN QUẢN TRỊ (Khách hàng không bao giờ được thấy cái này)
                Status = car.Status.ToString(), // Ép kiểu chữ cho dễ đọc
                car.IsDeleted,
                DeletedAt = car.DeletedAt?.ToString("dd/MM/yyyy HH:mm:ss"),
                car.DeletedBy,
                CreatedAt = car.CreatedAt?.ToString("dd/MM/yyyy HH:mm:ss"),
                UpdatedAt = car.UpdatedAt?.ToString("dd/MM/yyyy HH:mm:ss")
            });
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutCar(int id, [FromForm] Car car)
        //{
        //    if (id != car.CarId) return BadRequest("ID không khớp.");

        //    // 1. LẤY DỮ LIỆU CŨ TỪ DATABASE ĐỂ SO SÁNH
        //    var existingCar = await _carRepo.GetCarByIdAsync(id);
        //    if (existingCar == null) return NotFound();

        //    try
        //    {
        //        // 2. GIỮ LẠI CÁC THÔNG TIN KHÔNG ĐƯỢC PHÉP SỬA QUA FORM
        //        car.CreatedAt = existingCar.CreatedAt; // Ngày tạo phải giữ nguyên
        //        car.UpdatedAt = DateTime.Now;          // Cập nhật ngày sửa mới nhất
        //        car.IsDeleted = existingCar.IsDeleted; // Trạng thái xóa giữ nguyên

        //        // 3. XỬ LÝ ẢNH (LOGIC QUAN TRỌNG)
        //        if (car.ImageFile != null && car.ImageFile.Length > 0)
        //        {
        //            // BƯỚC A: Xóa ảnh cũ trong thư mục (nếu có và không phải ảnh mặc định)
        //            if (!string.IsNullOrEmpty(existingCar.ImageUrl) && !existingCar.ImageUrl.Contains("default-car.jpg"))
        //            {
        //                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingCar.ImageUrl.TrimStart('/'));
        //                if (System.IO.File.Exists(oldFilePath))
        //                {
        //                    System.IO.File.Delete(oldFilePath);
        //                }
        //            }

        //            // BƯỚC B: Tải ảnh mới lên qua Utility
        //            car.ImageUrl = await FileHelper.UploadFileAsync(car.ImageFile, "Cars", car.Name);
        //        }
        //        else
        //        {
        //            // Nếu Admin không chọn ảnh mới, ta phải giữ lại cái link ảnh cũ
        //            car.ImageUrl = existingCar.ImageUrl;
        //        }

        //        // 4. GIAO CHO THỦ KHO CẬP NHẬT
        //        await _carRepo.UpdateCarAsync(car);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
        //    }

        //    return Ok(new { message = "Cập nhật xe thành công!", data = car });
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCar(int id, [FromForm] Car car)
        {
            // Kiểm tra xem ID trên link và ID trong dữ liệu có khớp nhau không

            if (id != car.CarId) return BadRequest("Dữ liệu ID không đồng nhất.");

            // 1. Tìm xe cũ trong kho để lấy thông tin ảnh cũ và ngày tạo
            var existingCar = await _carRepo.GetCarByIdAsync(id);
            if (existingCar == null) return NotFound("Không tìm thấy xe này.");

            try
            {
                // 2. Bảo vệ các thông tin hệ thống (Không cho phép sửa qua form này)

                car.Name = existingCar.Name;
                car.CreatedAt = existingCar.CreatedAt; // Giữ nguyên ngày tạo
                car.UpdatedAt = DateTime.Now;          // Đóng dấu ngày sửa
                car.IsDeleted = existingCar.IsDeleted; // Giữ nguyên trạng thái xóa
                car.DeletedAt = existingCar.DeletedAt; // Giữ nguyên lịch sử xóa
                car.DeletedBy = existingCar.DeletedBy; // Giữ nguyên người xóa

                // 3. Xử lý logic thay ảnh
                if (car.ImageFile != null && car.ImageFile.Length > 0)
                {
                    // A. Nếu có ảnh mới: Xóa ảnh cũ trên server (trừ ảnh mặc định) để dọn rác
                    if (!string.IsNullOrEmpty(existingCar.ImageUrl) && !existingCar.ImageUrl.Contains("default-car.jpg"))
                    {
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingCar.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }

                    // B. Tải ảnh mới lên
                    car.ImageUrl = await FileHelper.UploadFileAsync(car.ImageFile, "Cars", car.Name);
                }
                else
                {
                    // Nếu Admin không chọn ảnh mới: Giữ lại cái link ảnh cũ, nếu không sẽ bị null mất ảnh
                    car.ImageUrl = existingCar.ImageUrl;
                }

                // 4. Cập nhật vào DB
                await _carRepo.UpdateCarAsync(car);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi cập nhật: {ex.Message}");
            }

            return Ok(new { message = "Cập nhật thành công!", car });
        }

        // POST: api/admin/Cars
        //[HttpPost]
        //public async Task<ActionResult<Car>> PostCar([FromForm] Car car)
        //{
        //    // 1. TỰ ĐỘNG XỬ LÝ CÁC TRƯỜNG DỮ LIỆU (Ép hệ thống tự làm, phớt lờ data trên Swagger)
        //    car.CarId = 0; // Để số 0 để Entity Framework hiểu là Thêm Mới, SQL sẽ tự nhảy số 1, 2, 3...
        //    car.CreatedAt = DateTime.Now; // Tự động lấy giờ hiện tại trên máy Server
        //    car.UpdatedAt = DateTime.Now;

        //    // Vì là xe mới thêm nên chắc chắn chưa bị xóa
        //    car.IsDeleted = false;
        //    car.DeletedAt = null;
        //    car.DeletedBy = null;

        //    // 2. XỬ LÝ UPLOAD FILE ẢNH (Nếu Admin có chọn ảnh)
        //    if (car.ImageFile != null && car.ImageFile.Length > 0)
        //    {
        //        // 1. Tạo tên thư mục dựa trên Tên Xe (Xóa dấu cách để link không bị lỗi)
        //        string folderName = car.Name.Replace(" ", "_");

        //        // 2. Xây dựng đường dẫn: wwwroot/uploads/Cars/Ten_Xe
        //        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "Cars", folderName);

        //        // 3. Tự động tạo cây thư mục nếu chưa có
        //        if (!Directory.Exists(folderPath))
        //        {
        //            Directory.CreateDirectory(folderPath);
        //        }

        //        // 4. Tạo tên file và lưu
        //        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(car.ImageFile.FileName);
        //        var filePath = Path.Combine(folderPath, fileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await car.ImageFile.CopyToAsync(stream);
        //        }

        //        // 5. Lưu link vào DB (Link này FE sẽ dùng để hiển thị ảnh)
        //        car.ImageUrl = $"/uploads/Cars/{folderName}/{fileName}";
        //    }
        //    else
        //    {
        //        // Nếu Admin quên chọn ảnh, có thể gán một ảnh mặc định (Tùy chọn)
        //        car.ImageUrl = "/uploads/default-car.jpg";
        //    }

        //    // 3. GIAO CHO "ÔNG THỦ KHO" LƯU VÀO DATABASE
        //    await _carRepo.AddCarAsync(car);

        //    // Báo thành công và trả về thông tin chiếc xe vừa tạo
        //    return CreatedAtAction("GetCar", new { id = car.CarId }, car);
        //}

        [HttpPost]
        public async Task<ActionResult<Car>> PostCar([FromForm] Car car)
        {
            if (await _carRepo.CheckNameExistAsync(car.Name))
            {
                return BadRequest($"Tên xe '{car.Name}' đã tồn tại trong hệ thống. Vui lòng chọn tên khác!");
            }

            // 1. Setup thông tin hệ thống
            car.CarId = 0;
            car.CreatedAt = DateTime.Now;
            car.UpdatedAt = DateTime.Now;
            car.IsDeleted = false;
            car.DeletedAt = null;
            car.DeletedBy = null;
            car.Status = CarStatus.Draft; // Luôn mặc định là chưa mở bán

            // 2. Xử lý ảnh với logic if-else mạch lạc
            if (car.ImageFile != null && car.ImageFile.Length > 0)
            {
                        // Có file thì nhờ FileHelper đi giao hàng
                        car.ImageUrl = await FileHelper.UploadFileAsync(car.ImageFile, "Cars", car.Name);
                    }
            else
            {
                // Không có file thì chỉ định ảnh mặc định ngay và luôn
                car.ImageUrl = "/uploads/Cars/default-car.jpg";
            }

            // 3. Lưu xuống Database
            await _carRepo.AddCarAsync(car);

            return CreatedAtAction("GetCar", new { id = car.CarId }, car);
        }

        [HttpDelete("{id}/SoftDeleteCar")]
        public async Task<IActionResult> SoftDeleteCar(int id)
        {
            // 1. Tìm chiếc xe cần xóa
            var car = await _carRepo.GetCarByIdAsync(id);
            if (car == null) return NotFound("Không tìm thấy xe để xóa.");

            if (car.IsDeleted == true)
            {
                return BadRequest("Xe này đã nằm trong thùng rác rồi, không thể xóa thêm nữa!");
            }

            // 2. THỰC HIỆN "XÓA MỀM" (Chỉ cập nhật trạng thái, không xóa vật lý)
            car.IsDeleted = true;                  // Đánh dấu là đã bay màu
            car.DeletedAt = DateTime.Now;          // Ghi nhận THỜI GIAN xóa (Lúc này mới dùng tới)

            // Tạm thời gán bằng 1 (Đại diện cho Admin hệ thống). 
            // Sau này làm chức năng Đăng nhập xong, mình sẽ lấy ID của người đang login nhét vào đây!
            car.DeletedBy = 1;

            // 3. Cập nhật lại vào Database
            try
            {
                await _carRepo.UpdateCarAsync(car);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }

            return Ok(new { message = "Đã đưa xe vào thùng rác thành công!" });
        }

        // PUT: api/admin/Cars/1/restore
        [HttpPut("{id}/restore")]
        public async Task<IActionResult> RestoreCar(int id)
        {
            // 1. Tìm xe (kể cả xe đã bị xóa)
            var car = await _carRepo.GetCarByIdAsync(id);
            if (car == null) return NotFound("Không tìm thấy xe này.");

            if (car.IsDeleted == false)
            {
                return BadRequest("Xe này đang bán lên sàn rồi!");
            }

            // 2. LÀM PHÉP HỒI SINH
            car.IsDeleted = false;
            //car.DeletedAt = null;
            //car.DeletedBy = null;

            car.UpdatedAt = DateTime.Now; // Tự động lấy giờ hệ thống, Admin không thể can thiệp

            // (Tùy chọn) Ní có thể tự động đổi Status về Draft để Admin duyệt lại trước khi lên sàn
            // car.Status = CarStatus.Draft; 

            // 3. Cập nhật vào kho
            try
            {
                await _carRepo.UpdateCarAsync(car);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khôi phục: {ex.Message}");
            }

            return Ok(new { message = "Đã hồi sinh xe thành công, sẵn sàng lên sàn!" });
        }

        // DELETE: api/admin/Cars/5/hard-delete
        [HttpDelete("{id}/hard-delete")]
        public async Task<IActionResult> HardDeleteCar(int id)
        {
            var car = await _carRepo.GetCarByIdAsync(id);
            if (car == null) return NotFound("Không tìm thấy xe để xóa.");

            // BƯỚC 1: LOGIC BẢO VỆ "CHỐNG BẤM NHẦM"
            // Chỉ cho phép xóa vĩnh viễn nếu: Đã bị xóa mềm (Nằm trong thùng rác) HOẶC Đang là bản nháp (Draft)
            bool isTrash = car.IsDeleted == true;
            bool isDraft = car.Status == CarStatus.Draft; // Đảm bảo đã có using Models để gọi Enum

            if (!isTrash && !isDraft)
            {
                return BadRequest("Xe đang được bán công khai! Yêu cầu đưa vào thùng rác (xóa mềm) trước khi xóa vĩnh viễn.");
            }

            // BƯỚC 2: XÓA VẬT LÝ FILE ẢNH TRÊN SERVER ĐỂ GIẢI PHÓNG BỘ NHỚ
            if (!string.IsNullOrEmpty(car.ImageUrl) && !car.ImageUrl.Contains("default-car.jpg"))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", car.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath); // Xóa file ảnh gốc
                }

                // (Nâng cao) Nếu thư mục của xe đó trống, có thể dùng code xóa luôn thư mục tên xe
            }

            // BƯỚC 3: XÓA CỨNG KHỎI DATABASE
            try
            {
                // Gọi hàm xóa thật sự trong Repository
                await _carRepo.DeleteCarAsync(id);
            }
            catch (Exception ex)
            {
                // Thường lỗi này xảy ra nếu xe đã lỡ dính khóa ngoại (Foreign Key) với bảng Orders/Reviews
                return StatusCode(500, $"Không thể xóa do vướng dữ liệu liên quan: {ex.Message}");
            }

            return Ok(new { message = "Đã tiêu diệt chiếc xe vĩnh viễn khỏi vũ trụ!" });
        }

    }
}