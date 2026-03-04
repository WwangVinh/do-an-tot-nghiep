using CoreEntities.Models;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Services.Repositories;
using LogicBusiness.Utilities;
using Microsoft.AspNetCore.Http;

namespace LogicBusiness.Services.Admin
{
    public class CarAdminService : ICarAdminService
    {
        private readonly ICarRepository _carRepo;
        private readonly ICarImageRepository _imageRepo;

        public CarAdminService(ICarRepository carRepo, ICarImageRepository imageRepo)
        {
            _carRepo = carRepo;
            _imageRepo = imageRepo;
        }

        // 1. GET ALL
        public async Task<object> GetCarsAsync(string? search, CarStatus? status, bool? isDeleted, int page, int pageSize)
        {
            var result = await _carRepo.GetAdminCarsAsync(search, status, isDeleted, page, pageSize);
            var adminCars = result.Cars.Select(c => new {
                c.CarId,
                c.Name,
                c.Brand,
                c.Year,
                c.Price,
                c.ImageUrl,
                Condition = c.Condition.ToString(),
                Status = c.Status.ToString(),
                c.IsDeleted,
                CreatedAt = c.CreatedAt?.ToString("dd/MM/yyyy HH:mm"),
                UpdatedAt = c.UpdatedAt?.ToString("dd/MM/yyyy HH:mm")
            });

            return new
            {
                TotalItems = result.TotalCount,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize),
                Data = adminCars
            };
        }

        // 2. GET DETAIL (Đóng gói ảnh siêu cấp)
        public async Task<object?> GetCarDetailAsync(int id)
        {
            var car = await _carRepo.GetCarDetailForAdminAsync(id);
            if (car == null) return null;

            return new
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
                Condition = car.Condition.ToString(),
                Status = car.Status.ToString(),
                car.IsDeleted,
                car.CreatedAt,
                car.UpdatedAt,

                // Gom nhóm ảnh phụ
                GalleryImages = car.CarImages.Where(img => img.Is360Degree == false)
                    .GroupBy(img => img.ImageType)
                    .Select(group => new {
                        Category = group.Key,
                        Images = group.Select(i => new { i.CarImageId, i.Title, i.Description, i.ImageUrl, i.FileHash }).ToList()
                    }).ToList(),

                // Mảng ảnh 360
                Images360 = car.CarImages.Where(img => img.Is360Degree == true)
                    .Select(i => new { i.CarImageId, i.ImageUrl }).ToList()
            };
        }

        // 3. CREATE
        public async Task<(bool Success, string Message, Car? Data)> CreateCarAsync(Car car, IFormFile? imageFile)
        {
            if (await _carRepo.CheckNameExistAsync(car.Name))
                return (false, $"Tên xe '{car.Name}' đã tồn tại!", null);

            car.CarId = 0;
            car.CreatedAt = DateTime.Now;
            car.UpdatedAt = DateTime.Now;
            car.IsDeleted = false;
            car.Status = CarStatus.Draft;

            if (imageFile != null && imageFile.Length > 0)
                car.ImageUrl = await FileHelper.UploadFileAsync(imageFile, "Cars", car.Name);
            else
                car.ImageUrl = "/uploads/Cars/default-car.jpg";

            await _carRepo.AddCarAsync(car);
            return (true, "Thêm xe thành công!", car);
        }

        // 4. UPDATE
        public async Task<(bool Success, string Message, Car? Data)> UpdateCarAsync(int id, Car car)
        {
            var existingCar = await _carRepo.GetCarByIdAsync(id);
            if (existingCar == null) return (false, "Không tìm thấy xe.", null);

            car.Name = existingCar.Name;
            car.CreatedAt = existingCar.CreatedAt;
            car.UpdatedAt = DateTime.Now;
            car.IsDeleted = existingCar.IsDeleted;
            car.DeletedAt = existingCar.DeletedAt;
            car.DeletedBy = existingCar.DeletedBy;

            if (car.ImageFile != null && car.ImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingCar.ImageUrl) && !existingCar.ImageUrl.Contains("default-car.jpg") && !existingCar.ImageUrl.StartsWith("http"))
                {
                    string normalizedUrl = existingCar.ImageUrl.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", normalizedUrl);
                    if (File.Exists(oldPath)) File.Delete(oldPath);
                }
                car.ImageUrl = await FileHelper.UploadFileAsync(car.ImageFile, "Cars", car.Name);
            }
            else
            {
                car.ImageUrl = existingCar.ImageUrl;
            }

            await _carRepo.UpdateCarAsync(car);
            return (true, "Cập nhật thành công!", car);
        }

        // 5. UPLOAD NHIỀU ẢNH PHỤ (GALLERY) CÙNG LÚC + MÔ TẢ RIÊNG
        public async Task<(bool Success, string Message, object? Data)> UploadGalleryImagesAsync(int carId, List<IFormFile> files, List<string>? titles, List<string>? descriptions, string imageType)
        {
            var car = await _carRepo.GetCarByIdAsync(carId);
            if (car == null) return (false, "Không tìm thấy xe!", null);

            var uploadedImages = new List<CarImage>();
            int skippedCount = 0;
            var existingImages = await _imageRepo.GetCarImagesAsync(carId);

            // Chạy vòng lặp for để khớp Index giữa File và Description
            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (file.Length == 0) continue;

                string fileHash = FileHelper.GetFileHash(file);
                if (existingImages.Any(img => img.FileHash == fileHash))
                {
                    skippedCount++;
                    continue;
                }

                // Lấy mô tả tương ứng: Nếu FE gửi thiếu hoặc không gửi thì để null
                string? currentTitle = (titles != null && i < titles.Count) ? titles[i] : null;
                string? currentDesc = (descriptions != null && i < descriptions.Count) ? descriptions[i] : null;

                string imagePath = await FileHelper.UploadFileAsync(file, "Cars", car.Name);

                var carImage = new CarImage
                {
                    CarId = carId,
                    ImageUrl = imagePath,
                    Is360Degree = false,
                    IsMainImage = false,
                    ImageType = string.IsNullOrWhiteSpace(imageType) ? "Khác" : imageType.Trim(),
                    Title = currentTitle,
                    Description = currentDesc, // 👈 Lưu mô tả riêng vào đây
                    FileHash = fileHash,
                    CreatedAt = DateTime.Now
                };

                await _imageRepo.AddCarImageAsync(carImage);
                uploadedImages.Add(carImage);
                existingImages.Add(carImage);
            }

            var responseData = uploadedImages.Select(img => new {
                img.CarImageId,
                img.ImageUrl,
                img.ImageType,
                img.Title,
                img.Description,
                img.FileHash
            });

            string msg = $"Thêm thành công {uploadedImages.Count} ảnh loại '{imageType}'.";
            if (skippedCount > 0) msg += $" (Bỏ qua {skippedCount} ảnh trùng).";

            return (true, msg, responseData);
        }

        public async Task<bool> UpdateImageDetailsAsync(int imageId, string? title, string? description)
        {
            // Gọi thẳng xuống Thủ kho (Repository)
            // Lưu ý: Lúc nãy mình viết tham số ở Repo là (imageId, description, title) 
            return await _imageRepo.UpdateImageDescriptionAsync(imageId, description, title);
        }

        // 6. UPLOAD 360
        public async Task<(bool Success, string Message, object? Data)> Upload360ImagesAsync(int carId, List<IFormFile> files)
        {
            var car = await _carRepo.GetCarByIdAsync(carId);
            if (car == null) return (false, "Không tìm thấy xe.", null);

            string folderName = car.Name.Replace(" ", "_");
            string subFolder = Path.Combine("Cars", folderName, "360");
            var uploadedImages = new List<CarImage>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string imagePath = await FileHelper.UploadFileAsync(file, subFolder, "exterior");
                    var carImage = new CarImage
                    {
                        CarId = carId,
                        ImageUrl = imagePath,
                        Is360Degree = true,
                        IsMainImage = false,
                        CreatedAt = DateTime.Now
                    };
                    await _imageRepo.AddCarImageAsync(carImage);
                    uploadedImages.Add(carImage);
                }
            }
            var responseData = uploadedImages.Select(img => new { img.CarImageId, img.ImageUrl, img.Is360Degree });
            return (true, "Tải ảnh 360 thành công", responseData);
        }

        // 7. DELETE IMAGE
        public async Task<bool> DeleteCarImageAsync(int imageId)
        {
            var image = await _imageRepo.GetCarImageByIdAsync(imageId);
            if (image == null) return false;

            if (!string.IsNullOrEmpty(image.ImageUrl) && !image.ImageUrl.Contains("default-car.jpg") && !image.ImageUrl.StartsWith("http"))
            {
                string normalizedUrl = image.ImageUrl.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", normalizedUrl);
                if (File.Exists(filePath)) File.Delete(filePath);
            }

            await _imageRepo.DeleteCarImageAsync(imageId);
            return true;
        }

        // 8. SOFT DELETE
        public async Task<bool> SoftDeleteCarAsync(int id, int deletedByUserId)
        {
            var car = await _carRepo.GetCarByIdAsync(id);
            if (car == null || car.IsDeleted == true) return false;

            car.IsDeleted = true;
            car.DeletedAt = DateTime.Now;
            car.DeletedBy = deletedByUserId;
            await _carRepo.UpdateCarAsync(car);
            return true;
        }

        // 9. RESTORE
        public async Task<bool> RestoreCarAsync(int id)
        {
            var car = await _carRepo.GetCarByIdAsync(id);
            if (car == null || car.IsDeleted == false) return false;

            car.IsDeleted = false;
            car.UpdatedAt = DateTime.Now;
            await _carRepo.UpdateCarAsync(car);
            return true;
        }

        // 10. HARD DELETE
        public async Task<bool> HardDeleteCarAsync(int id)
        {
            var car = await _carRepo.GetCarByIdAsync(id);
            if (car == null) return false;

            if (car.IsDeleted != true && car.Status != CarStatus.Draft) return false;

            // A. Xóa Folder vật lý (Giữ nguyên code của ní)
            string folderName = car.Name.Replace(" ", "_");
            var carFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "Cars", folderName);

            if (Directory.Exists(carFolderPath))
            {
                Directory.Delete(carFolderPath, true);
            }
            else if (!string.IsNullOrEmpty(car.ImageUrl) && !car.ImageUrl.Contains("default-car.jpg"))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", car.ImageUrl.TrimStart('/'));
                if (File.Exists(imagePath)) File.Delete(imagePath);
            }

            // B. XÓA DỮ LIỆU TRONG BẢNG CAR IMAGES TRƯỚC 👈 (Chèn thêm dòng này)
            await _imageRepo.DeleteAllImagesByCarIdAsync(id);

            // C. CUỐI CÙNG MỚI XÓA XE
            await _carRepo.DeleteCarAsync(id);

            return true;
        }
    }
}