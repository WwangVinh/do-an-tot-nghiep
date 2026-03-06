using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Services.Repositories;
using LogicBusiness.Utilities;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
//using LogicBusiness.Services.Repositories;

namespace LogicBusiness.Services.Admin
{
    public class CarAdminService : ICarAdminService
    {
        private readonly ICarRepository _carRepo;
        private readonly ICarImageRepository _imageRepo;
        private readonly ICarFeatureRepository _carFeatureRepo;             // Gọi đệ tử 1
        private readonly ICarSpecificationRepository _carSpecificationRepo; // Gọi đệ tử 2

        public CarAdminService(ICarRepository carRepo, ICarImageRepository imageRepo, ICarFeatureRepository carFeatureRepo, ICarSpecificationRepository carSpecificationRepo)
        {
            _carRepo = carRepo;
            _imageRepo = imageRepo;
            _carFeatureRepo = carFeatureRepo;
            _carSpecificationRepo = carSpecificationRepo;
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

        // 2. GET DETAIL (Bản Full Options 2026)
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

                // 1. NHÓM THÔNG SỐ KỸ THUẬT (Đúng kiểu "Nhóm hóa" ní muốn)
                Specifications = car.CarSpecifications
                    .GroupBy(s => s.Category)
                    .Select(group => new {
                        Category = group.Key,
                        Items = group.Select(i => new { i.SpecName, i.SpecValue }).ToList()
                    }).ToList(),

                // 2. DANH SÁCH TIỆN ÍCH (Sửa lại cho khớp với Model Feature)
                Features = car.CarFeatures
                    .Select(cf => new {
                        cf.FeatureId,
                        // Sửa Name thành FeatureName cho đúng với file Feature.cs
                        FeatureName = cf.Feature?.FeatureName,
                        // Tiện tay ní lấy luôn cái Icon để sau này hiển thị cho đẹp
                        Icon = cf.Feature?.Icon
                    }).ToList(),

                // 3. GOM NHÓM ẢNH PHỤ (Đã có sẵn logic của ní)
                GalleryImages = car.CarImages.Where(img => img.Is360Degree == false)
                    .GroupBy(img => img.ImageType)
                    .Select(group => new {
                        Category = group.Key,
                        Images = group.Select(i => new { i.CarImageId, i.Title, i.Description, i.ImageUrl }).ToList()
                    }).ToList(),

                // 4. MẢNG ẢNH 360 (Đã có sẵn logic của ní)
                Images360 = car.CarImages.Where(img => img.Is360Degree == true)
                    .Select(i => new { i.CarImageId, i.ImageUrl }).ToList()
            };
        }

        // 3. CREATE
        //public async Task<(bool Success, string Message, Car? Data)> CreateCarAsync(Car car, IFormFile? imageFile)
        //{
        //    if (await _carRepo.CheckNameExistAsync(car.Name))
        //        return (false, $"Tên xe '{car.Name}' đã tồn tại!", null);

        //    car.CarId = 0;
        //    car.CreatedAt = DateTime.Now;
        //    car.UpdatedAt = DateTime.Now;
        //    car.IsDeleted = false;
        //    car.Status = CarStatus.Draft;

        //    if (imageFile != null && imageFile.Length > 0)
        //        car.ImageUrl = await FileHelper.UploadFileAsync(imageFile, "Cars", car.Name);
        //    else
        //        car.ImageUrl = "/uploads/Cars/default-car.jpg";

        //    await _carRepo.AddCarAsync(car);
        //    return (true, "Thêm xe thành công!", car);
        //}
        public async Task<(bool Success, string Message, Car? Data)> CreateCarAsync(CarCreateDto dto)
        {
            // 1. KIỂM TRA TÊN XE
            if (await _carRepo.CheckNameExistAsync(dto.Name))
                return (false, $"Tên xe '{dto.Name}' đã tồn tại!", null);

            // 2. TẠO THÔNG TIN CƠ BẢN CỦA XE
            var car = new Car
            {
                Name = dto.Name,
                Brand = dto.Brand,
                Year = dto.Year,
                Model = dto.Model,             // Nhận vào đây
                Color = dto.Color,             // Nhận vào đây
                Condition = dto.Condition,
                Price = dto.Price,
                FuelType = dto.FuelType,       // Nhận vào đây
                Mileage = dto.Mileage ?? 0,    // Nhận vào đây
                Description = dto.Description, // Nhận vào đây

                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsDeleted = false,
                Status = CarStatus.Draft // Tùy ní xài Enum hay string
            };

            // Xử lý ảnh xe
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
                car.ImageUrl = await FileHelper.UploadFileAsync(dto.ImageFile, "Cars", car.Name);
            else
                car.ImageUrl = "/uploads/Cars/default-car.jpg.png"; // Ảnh mặc định

            // 👉 LƯU XE LẦN 1: Để hệ thống sinh ra cái CarId mới tinh
            await _carRepo.AddCarAsync(car);

            // -------------------------------------------------------------
            // TỪ ĐÂY TRỞ XUỐNG BẮT ĐẦU XỬ LÝ 2 BẢNG PHỤ DỰA VÀO CARID MỚI
            // -------------------------------------------------------------

            // 3. THÊM TÍNH NĂNG (FEATURES)
            if (!string.IsNullOrWhiteSpace(dto.FeatureIds))
            {
                // Tuyệt kỹ: Tách chuỗi bằng dấu phẩy, dọn khoảng trắng, và biến thành list số nguyên
                var fIds = dto.FeatureIds.Split(',')
                                         .Select(id => id.Trim())
                                         .Where(id => int.TryParse(id, out _)) // Lọc bỏ nếu lỡ gõ bậy chữ cái
                                         .Select(int.Parse)
                                         .ToList();

                if (fIds.Any())
                {
                    var carFeatures = fIds.Select(fId => new CarFeature
                    {
                        CarId = car.CarId,
                        FeatureId = fId
                    });
                    await _carFeatureRepo.AddRangeAsync(carFeatures);
                }
            }

            //// 4. THÊM THÔNG SỐ KỸ THUẬT (BẢN VÉT CẠN)
            //if (!string.IsNullOrWhiteSpace(dto.SpecificationsJson))
            //{
            //    try
            //    {
            //        var rawStr = dto.SpecificationsJson.Trim();
            //        int startIdx = rawStr.IndexOf('[');
            //        int endIdx = rawStr.LastIndexOf(']');

            //        if (startIdx != -1 && endIdx != -1)
            //        {
            //            var cleanJson = rawStr.Substring(startIdx, endIdx - startIdx + 1).Replace("\\\"", "\"");

            //            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            //            var categories = System.Text.Json.JsonSerializer.Deserialize<List<SpecCategoryDto>>(cleanJson, options);

            //            if (categories != null && categories.Any())
            //            {
            //                var carSpecs = new List<CarSpecification>();
            //                foreach (var cat in categories)
            //                {
            //                    if (cat.Items == null) continue; // Chặn lỗi nếu Items rỗng

            //                    foreach (var item in cat.Items)
            //                    {
            //                        carSpecs.Add(new CarSpecification
            //                        {
            //                            CarId = car.CarId,
            //                            Category = cat.Category ?? "Thông số chung",
            //                            SpecName = item.Name,  // Đảm bảo item có thuộc tính Name
            //                            SpecValue = item.Value // Đảm bảo item có thuộc tính Value
            //                        });
            //                    }
            //                }

            //                if (carSpecs.Any())
            //                {
            //                    await _carSpecificationRepo.AddRangeAsync(carSpecs);
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        // Nếu tạch, nó sẽ hiện lỗi ở đây trên Swagger
            //        return (false, $"Lỗi xử lý JSON: {ex.Message}", car);
            //    }
            //}

           
            // 4. THÊM THÔNG SỐ KỸ THUẬT (KIỂU CHUỖI PHÂN CÁCH)
            if (!string.IsNullOrWhiteSpace(dto.Specifications))
            {
                try
                {
                    var specLines = dto.Specifications.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    var carSpecs = new List<CarSpecification>();

                    foreach (var line in specLines)
                    {
                        var parts = line.Split('|');
                        if (parts.Length == 3) // Đúng định dạng: Nhóm|Tên|Giá trị
                        {
                            carSpecs.Add(new CarSpecification
                            {
                                CarId = car.CarId, // CarId lấy từ con xe vừa tạo phía trên
                                Category = parts[0].Trim(),
                                SpecName = parts[1].Trim(),
                                SpecValue = parts[2].Trim()
                            });
                        }
                    }

                    if (carSpecs.Any())
                    {
                        await _carSpecificationRepo.AddRangeAsync(carSpecs); // Nhớ kiểm tra SaveChanges trong Repo nhé!
                    }
                    else
                    {
                        return (false, "Lỗi: Chuỗi thông số không đúng định dạng 'Nhóm|Tên|Giá trị;...'", car);
                    }
                }
                catch (Exception ex)
                {
                    return (false, $"Lỗi khi xử lý thông số: {ex.Message}", car);
                }
            }

            // THÀNH CÔNG RỰC RỠ!
            return (true, "Đã thêm xe, tính năng và thông số kỹ thuật thành công!", car);
        }

        // 4. UPDATE (Bản Chốt Hạ 2026)
        public async Task<(bool Success, string Message, Car? Car)> UpdateCarAsync(int id, CarUpdateDto dto)
        {
            // 1. Tìm xe cũ trong SQL Server 2022
            var car = await _carRepo.GetByIdAsync(id);
            if (car == null) return (false, "Không tìm thấy xe!", null);

            // 2. Cập nhật thông tin cơ bản
            car.Name = dto.Name;
            car.Brand = dto.Brand;
            car.Model = dto.Model;
            car.Price = (decimal)dto.Price;
            car.Year = dto.Year;
            car.Description = dto.Description;
            car.UpdatedAt = DateTime.Now;

            // Truyền cả tên và ID của con xe hiện tại
            if (await _carRepo.CheckNameExistAsync(dto.Name, id))
                return (false, "Tên này đã bị một con xe khác chiếm dụng mất rồi!", null);

            try
            {
                // 3. XỬ LÝ ẢNH CHÍNH (Bản nâng cấp theo cấu trúc Thư mục Hãng)
                if (dto.ImageFile != null)
                {
                    try
                    {
                        // A. Xác định thư mục lưu theo Brand (Ví dụ: "Cars/BMW")
                        string brandFolder = !string.IsNullOrEmpty(dto.Brand) ? $"Cars/{dto.Brand}" : "Cars/Others";

                        // B. Lưu ảnh mới vào đúng thư mục
                        string newImageUrl = await _imageRepo.UploadImageAsync(dto.ImageFile, brandFolder);

                        // C. XÓA ẢNH CŨ (Tránh rác server)
                        if (!string.IsNullOrEmpty(car.ImageUrl) && !car.ImageUrl.Contains("default-car.jpg"))
                        {
                            // Chuyển đường dẫn web (/uploads/...) thành đường dẫn vật lý (C:\...)
                            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", car.ImageUrl.TrimStart('/'));

                            if (File.Exists(oldPath))
                            {
                                File.Delete(oldPath);
                            }
                        }

                        // D. Cập nhật đường dẫn mới vào database
                        car.ImageUrl = newImageUrl;
                    }
                    catch (Exception ex)
                    {
                        return (false, $"Lỗi khi thay ảnh chính: {ex.Message}", null);
                    }
                }

                // 4. XỬ LÝ THÔNG SỐ KỸ THUẬT (Chiến thuật Xóa sạch - Xây mới)
                // A. Quét sạch bãi chiến trường cũ để tránh trùng lặp
                await _carSpecificationRepo.DeleteByCarIdAsync(id);

                // B. Nạp mớ "nồi lẩu" mới từ chuỗi phân cách
                if (!string.IsNullOrWhiteSpace(dto.Specifications))
                {
                    var specLines = dto.Specifications.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    var newSpecs = new List<CarSpecification>();

                    foreach (var line in specLines)
                    {
                        var parts = line.Split('|');
                        if (parts.Length == 3)
                        {
                            newSpecs.Add(new CarSpecification
                            {
                                CarId = id,
                                Category = parts[0].Trim(),
                                SpecName = parts[1].Trim(),
                                SpecValue = parts[2].Trim()
                            });
                        }
                    }

                    if (newSpecs.Any())
                    {
                        await _carSpecificationRepo.AddRangeAsync(newSpecs);
                    }
                }

                // 5. XỬ LÝ TIỆN ÍCH (Features: ABS, Cửa sổ trời...)
                try
                {
                    // Bước A: Xóa sạch các tiện ích cũ của xe này trong bảng trung gian
                    // Ní dùng hàm đã khai báo trong Interface ở hình image_7cd2c2.jpg
                    await _carFeatureRepo.DeleteByCarIdAsync(id);

                    // Bước B: Nếu chuỗi FeatureIds gửi lên có dữ liệu (VD: "1,2,3")
                    if (!string.IsNullOrWhiteSpace(dto.FeatureIds))
                    {
                        // Tách chuỗi thành danh sách số nguyên
                        var featureIds = dto.FeatureIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                     .Select(int.Parse)
                                                     .ToList();

                        // Tạo danh sách các đối tượng CarFeature mới
                        var newCarFeatures = featureIds.Select(fId => new CarFeature
                        {
                            CarId = id,
                            FeatureId = fId
                        }).ToList();

                        // Lưu vào Database thông qua Repository
                        await _carFeatureRepo.AddRangeAsync(newCarFeatures);
                    }
                }
                catch (Exception ex)
                {
                    return (false, $"Lỗi khi cập nhật tiện ích: {ex.Message}", null);
                }

                // 5. Chốt đơn lưu xe
                await _carRepo.UpdateAsync(car);

                return (true, "Cập nhật xe, ảnh và thông số thành công!", car);
        }
            catch (Exception ex)
            {
                return (false, $"Lỗi hệ thống: {ex.Message}", null);
            }
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
            await _carFeatureRepo.DeleteByCarIdAsync(id);
            await _carSpecificationRepo.DeleteByCarIdAsync(id);


            // C. CUỐI CÙNG MỚI XÓA XE
            await _carRepo.DeleteCarAsync(id);

            return true;
        }

        //public async Task<bool> HardDeleteCarAsync(int id)
        //{
        //    // 1. Dọn dẹp bảng Tính năng (CarFeatures)
        //    await _carFeatureRepo.DeleteByCarIdAsync(id);

        //    // 2. Dọn dẹp bảng Thông số kỹ thuật (CarSpecifications)
        //    await _carSpecificationRepo.DeleteByCarIdAsync(id);

        //    // (Nếu ní có bảng CarImages thì gọi Repo xóa hình luôn ở bước 3 này nha)
        //    // await _carImageRepo.DeleteByCarIdAsync(id); 

        //    // 4. Khi đám "con nheo nhóc" đã bị dọn sạch, giờ thì tử hình thằng "Cha" (Car)
        //    return await _carRepo.HardDeleteCarAsync(id);
        //}
    }
}