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
        private readonly OtoContext _context;
        private readonly ICarRepository _carRepo; // Dùng thủ kho dể lọc xe
        private readonly ICarImageRepository _imageRepo; // khai báo thêm thủ kho CarImages để sau này còn có ảnh 360

        // Hàm tạo mã băm (Vân tay) cho file ảnh để chống trùng
        private string GetFileHash(IFormFile file)
        {
            using var md5 = System.Security.Cryptography.MD5.Create();
            using var stream = file.OpenReadStream();
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
        public CarsController(OtoContext context, ICarRepository carRepo,[FromForm] ICarImageRepository imageRepo)
        {
            _context = context;
            _carRepo = carRepo;
            _imageRepo = imageRepo; // 3. Gán giá trị
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
        // GET: api/admin/Cars/1024
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarDetailForAdmin(int id)
        {
            // 1. Lấy Full thông tin xe dành riêng cho quyền Admin
            var car = await _carRepo.GetCarDetailForAdminAsync(id);

            if (car == null)
            {
                return NotFound(new { message = "Không tìm thấy xe này trong hệ thống!" });
            }

            try
            {
                // 2. ĐÓNG GÓI DỮ LIỆU SIÊU CẤP CHO GIAO DIỆN ADMIN
                var carDetail = new
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
                    Status = car.Status.ToString(),
                    car.IsDeleted,
                    car.CreatedAt,
                    car.UpdatedAt,

                    // BÍ KÍP 1: Gom nhóm ảnh phụ theo ImageType (Nội thất, Ngoại thất...)
                    GalleryImages = car.CarImages
                        .Where(img => img.Is360Degree == false)
                        .GroupBy(img => img.ImageType)
                        .Select(group => new {
                            Category = group.Key, // Tên thư mục (VD: "Nội thất")
                            Images = group.Select(i => new {
                                i.CarImageId, // BẮT BUỘC PHẢI CÓ để Admin còn gọi API Xóa
                                i.ImageUrl,
                                i.FileHash
                            }).ToList()
                        }).ToList(),

                    // BÍ KÍP 2: Lấy mảng ảnh 360
                    Images360 = car.CarImages
                        .Where(img => img.Is360Degree == true)
                        .Select(i => new {
                            i.CarImageId, // BẮT BUỘC PHẢI CÓ để Admin còn gọi API Xóa
                            i.ImageUrl
                        }).ToList()
                };

                return Ok(carDetail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        // ================= KHU VỰC QUẢN LÝ ẢNH PHỤ =================

        // POST: api/admin/Cars/5/images
        [HttpPost("{carId}/images")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadCarImage([FromRoute] int carId, IFormFile file, [FromForm] string imageType) // 👈 Nhận thêm imageType
        {
            var car = await _carRepo.GetCarByIdAsync(carId);
            if (car == null) return NotFound("Không tìm thấy xe để thêm ảnh!");

            if (file == null || file.Length == 0) return BadRequest("Vui lòng chọn một file ảnh!");

            try
            {
                // 1. TẠO VÂN TAY VÀ KIỂM TRA TRÙNG LẶP
                string fileHash = GetFileHash(file);

                // Vào DB lục xem chiếc xe này đã có bức ảnh nào mang vân tay này chưa
                bool isDuplicate = await _context.CarImages.AnyAsync(img => img.CarId == carId && img.FileHash == fileHash);
                if (isDuplicate)
                {
                    return BadRequest("Tấm ảnh này đã được tải lên rồi, vui lòng chọn ảnh khác!"); // 👈 Chặn đứng spam click!
                }

                // 2. Tải ảnh lên (Ổ cứng cứ cho vào chung folder Cars/TenXe là đủ)
                string imagePath = await FileHelper.UploadFileAsync(file, "Cars", car.Name);

                // 3. Tạo Object CarImage để lưu vào DB
                var carImage = new CarImage
                {
                    CarId = carId,
                    ImageUrl = imagePath,
                    Is360Degree = false,
                    IsMainImage = false,
                    ImageType = string.IsNullOrWhiteSpace(imageType) ? "Khác" : imageType.Trim(), // 👈 Dán nhãn phân loại
                    FileHash = fileHash, // 👈 Lưu lại vân tay để lần sau còn chặn
                    CreatedAt = DateTime.Now
                };

                await _imageRepo.AddCarImageAsync(carImage);

                return Ok(new
                {
                    message = $"Thêm ảnh phụ loại '{carImage.ImageType}' thành công!",
                    data = carImage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi upload ảnh: {ex.Message}");
            }
        }

        // POST: api/admin/Cars/5/upload-360
        //[HttpPost("{carId}/upload-360")]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> Upload360Image([FromRoute] int carId,  IFormFile file)
        //{
        //    // 1. Tìm xe để lấy thông tin tên xe làm thư mục cha
        //    var car = await _carRepo.GetCarByIdAsync(carId);
        //    if (car == null) return NotFound("Không tìm thấy xe này.");

        //    if (file == null || file.Length == 0) return BadRequest("Vui lòng chọn file ảnh 360.");

        //    try
        //    {
        //        // 2. XỬ LÝ ĐƯỜNG DẪN THƯ MỤC CON: "Cars/Ten_Xe/360"
        //        string folderName = car.Name.Replace(" ", "_");
        //        string subFolder = Path.Combine("Cars", folderName, "360");

        //        // 3. Giao cho FileHelper lưu vào đúng folder "360"
        //        string imagePath = await FileHelper.UploadFileAsync(file, subFolder, "exterior");

        //        // 4. Tạo Object để lưu vào Database
        //        var carImage = new CarImage
        //        {
        //            CarId = carId,
        //            ImageUrl = imagePath, // TUI MỞ KHÓA DÒNG NÀY RỒI NHA, NẾU KHÔNG LÀ MẤT LINK ĐÓ
        //            Is360Degree = true, // Luôn đánh dấu là ảnh 360 độ
        //            CreatedAt = DateTime.Now
        //        };

        //        // 5. Lưu vào Repository chuyên biệt cho Ảnh
        //        await _imageRepo.AddCarImageAsync(carImage);

        //        return Ok(new
        //        {
        //            message = $"Đã tải lên ảnh 360 vào thư mục {folderName}/360 thành công!",
        //            data = new
        //            {
        //                CarId = carImage.CarId,
        //                ImageUrl = carImage.ImageUrl,
        //                Is360Degree = carImage.Is360Degree,
        //                CreatedAt = carImage.CreatedAt
        //            } // TẠO OBJECT MỚI CHỈ CHỨA DỮ LIỆU CƠ BẢN, CẮT ĐỨT VÒNG LẶP
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Lỗi hệ thống khi xử lý ảnh 360: {ex.Message}");
        //    }
        //}

        [HttpPost("{id}/upload-360")]
        [Consumes("multipart/form-data")]
        // BÍ KÍP ĐỔI THÀNH List<IFormFile> ĐỂ CHỌN NHIỀU ẢNH CÙNG LÚC
        public async Task<IActionResult> Upload360Image([FromRoute] int id, [FromForm] List<IFormFile> files)
        {
            var car = await _carRepo.GetCarByIdAsync(id);
            if (car == null) return NotFound("Không tìm thấy xe này.");

            if (files == null || files.Count == 0) return BadRequest("Vui lòng chọn ít nhất 1 file ảnh 360.");

            try
            {
                string folderName = car.Name.Replace(" ", "_");
                string subFolder = Path.Combine("Cars", folderName, "360");
                var uploadedImages = new List<CarImage>();

                // DÙNG VÒNG LẶP ĐỂ XỬ LÝ TỪNG ẢNH TRONG DANH SÁCH 
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        // 1. Lưu file vào thư mục
                        string imagePath = await FileHelper.UploadFileAsync(file, subFolder, "exterior");

                        // 2. Tạo Object
                        var carImage = new CarImage
                        {
                            CarId = id,
                            ImageUrl = imagePath,
                            Is360Degree = true,
                            IsMainImage = false,
                            CreatedAt = DateTime.Now
                        };

                        // 3. Lưu vào DB
                        await _imageRepo.AddCarImageAsync(carImage);

                        // Add vào list để lát trả về kết quả cho FE biết đã up những ảnh nào
                        uploadedImages.Add(carImage);
                    }
                }

                return Ok(new
                {
                    message = $"Đã tải lên {files.Count} ảnh 360 thành công!",
                    // Trả về danh sách ảnh vừa up (đã cắt vòng lặp an toàn)
                    data = uploadedImages.Select(img => new {
                        img.CarImageId,
                        img.ImageUrl,
                        img.Is360Degree
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        // DELETE: api/admin/Cars/delete-image/5
        [HttpDelete("delete-image/{imageId}")]
        public async Task<IActionResult> DeleteCarImage(int imageId)
        {
            // 1. Tìm ảnh trong Kho
            var image = await _imageRepo.GetCarImageByIdAsync(imageId); // Đảm bảo Thủ kho Image có hàm này nha
            if (image == null) return NotFound("Không tìm thấy ảnh này trong hệ thống.");

            try
            {
                // 2. Xóa file vật lý trên Server (Trang bị bộ lọc đường dẫn xịn xò)
                if (!string.IsNullOrEmpty(image.ImageUrl) &&
                    !image.ImageUrl.Contains("default-car.jpg") &&
                    !image.ImageUrl.StartsWith("http"))
                {
                    string normalizedUrl = image.ImageUrl
                        .TrimStart('/', '\\')
                        .Replace('/', Path.DirectorySeparatorChar)
                        .Replace('\\', Path.DirectorySeparatorChar);

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", normalizedUrl);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                // 3. Xóa bản ghi trong Database
                await _imageRepo.DeleteCarImageAsync(imageId); // Đảm bảo Thủ kho Image có hàm xóa này

                return Ok(new { message = "Đã xóa ảnh vĩnh viễn thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi xóa file ảnh: {ex.Message}");
            }
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

            // Khai báo sẵn câu thông báo mặc định
            string responseMessage = "Cập nhật thông tin xe thành công!";

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
                    // A. Nếu có ảnh mới: Xóa ảnh cũ trên server (trừ ảnh mặc định) để dọn rác
                    // Bỏ qua nếu link là default-car.jpg hoặc link web (http)
                    if (!string.IsNullOrEmpty(existingCar.ImageUrl) &&
                        !existingCar.ImageUrl.Contains("default-car.jpg") &&
                        !existingCar.ImageUrl.StartsWith("http"))
                    {
                        // 1. Cạo sạch cả '/' lẫn '\' ở đầu chuỗi
                        // 2. Đồng bộ toàn bộ dấu gạch cho chuẩn với hệ điều hành đang chạy (Windows/Mac)
                        string normalizedUrl = existingCar.ImageUrl
                            .TrimStart('/', '\\')
                            .Replace('/', Path.DirectorySeparatorChar)
                            .Replace('\\', Path.DirectorySeparatorChar);

                        // 3. Ghép đường dẫn an toàn 100%
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", normalizedUrl);

                        // 4. Tìm thấy thì tiêu diệt vật lý
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    // B. Tải ảnh mới lên
                    car.ImageUrl = await FileHelper.UploadFileAsync(car.ImageFile, "Cars", car.Name);

                    // Đổi lời nhắn vì Admin có thay đổi cả ảnh đại diện
                    responseMessage = "Cập nhật thông tin và thay đổi ảnh đại diện mới thành công!";
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

            // Trả về câu thông báo đã được điều chỉnh linh hoạt
            return Ok(new { message = responseMessage, car });
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

        //// DELETE: api/admin/Cars/5/hard-delete
        //[HttpDelete("{id}/hard-delete")]
        //public async Task<IActionResult> HardDeleteCar(int id)
        //{
        //    var car = await _carRepo.GetCarByIdAsync(id);
        //    if (car == null) return NotFound("Không tìm thấy xe để xóa.");

        //    // BƯỚC 1: LOGIC BẢO VỆ "CHỐNG BẤM NHẦM"
        //    // Chỉ cho phép xóa vĩnh viễn nếu: Đã bị xóa mềm (Nằm trong thùng rác) HOẶC Đang là bản nháp (Draft)
        //    bool isTrash = car.IsDeleted == true;
        //    bool isDraft = car.Status == CarStatus.Draft; // Đảm bảo đã có using Models để gọi Enum

        //    if (!isTrash && !isDraft)
        //    {
        //        return BadRequest("Xe đang được bán công khai! Yêu cầu đưa vào thùng rác (xóa mềm) trước khi xóa vĩnh viễn.");
        //    }

        //    // BƯỚC 2: XÓA VẬT LÝ FILE ẢNH TRÊN SERVER ĐỂ GIẢI PHÓNG BỘ NHỚ
        //    if (!string.IsNullOrEmpty(car.ImageUrl) && !car.ImageUrl.Contains("default-car.jpg"))
        //    {
        //        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", car.ImageUrl.TrimStart('/'));
        //        if (System.IO.File.Exists(imagePath))
        //        {
        //            System.IO.File.Delete(imagePath); // Xóa file ảnh gốc
        //        }

        //        // (Nâng cao) Nếu thư mục của xe đó trống, có thể dùng code xóa luôn thư mục tên xe
        //    }

        //    // BƯỚC 3: XÓA CỨNG KHỎI DATABASE
        //    try
        //    {
        //        // Gọi hàm xóa thật sự trong Repository
        //        await _carRepo.DeleteCarAsync(id);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Thường lỗi này xảy ra nếu xe đã lỡ dính khóa ngoại (Foreign Key) với bảng Orders/Reviews
        //        return StatusCode(500, $"Không thể xóa do vướng dữ liệu liên quan: {ex.Message}");
        //    }

        //    return Ok(new { message = "Đã tiêu diệt chiếc xe vĩnh viễn khỏi vũ trụ!" });
        //}

        // DELETE: api/admin/Cars/5/hard-delete
        [HttpDelete("{id}/hard-delete")]
        public async Task<IActionResult> HardDeleteCar(int id)
        {
            var car = await _carRepo.GetCarByIdAsync(id);
            if (car == null) return NotFound("Không tìm thấy xe để xóa.");

            // 1. LOGIC BẢO VỆ "CHỐNG BẤM NHẦM"
            bool isTrash = car.IsDeleted == true;
            bool isDraft = car.Status == CarStatus.Draft;

            if (!isTrash && !isDraft)
            {
                return BadRequest("Xe đang được bán công khai! Yêu cầu đưa vào thùng rác (xóa mềm) trước khi xóa vĩnh viễn.");
            }

            try
            {
                // 2. XÓA SẠCH VẬT LÝ TOÀN BỘ THƯ MỤC CỦA XE (Bao gồm ảnh chính, ảnh phụ, folder 360)
                // Thay vì xóa từng file, ta xóa luôn gốc rễ thư mục mang tên xe đó
                string folderName = car.Name.Replace(" ", "_");
                var carFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "Cars", folderName);

                if (Directory.Exists(carFolderPath))
                {
                    // Tham số 'true' có nghĩa là xóa sạch mọi thứ (file, folder con 360) bên trong nó
                    Directory.Delete(carFolderPath, true);
                }
                else if (!string.IsNullOrEmpty(car.ImageUrl) && !car.ImageUrl.Contains("default-car.jpg"))
                {
                    // Đề phòng trường hợp xe cũ không có folder riêng mà chỉ có file ảnh lẻ
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", car.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);
                }

                // 3. XÓA ẢNH TRONG BẢNG CarImages TRƯỚC (Để tránh lỗi khóa ngoại)
                // (Nếu Database của ní có cài đặt ON DELETE CASCADE thì không cần dòng này, nhưng gọi qua Repo cho chắc)
                var carImages = await _context.CarImages.Where(img => img.CarId == id).ToListAsync();
                if (carImages.Any())
                {
                    _context.CarImages.RemoveRange(carImages);
                    await _context.SaveChangesAsync();
                }

                // 4. XÓA CỨNG XE KHỎI DATABASE
                await _carRepo.DeleteCarAsync(id);

                return Ok(new { message = "Đã tiêu diệt chiếc xe và dọn sạch ổ cứng vĩnh viễn khỏi vũ trụ!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Không thể xóa do vướng dữ liệu liên quan: {ex.Message}");
            }
        }

    }
}