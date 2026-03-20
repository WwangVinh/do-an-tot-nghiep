using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;
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
        private readonly ICarFeatureRepository _carFeatureRepo;             
        private readonly ICarSpecificationRepository _carSpecificationRepo;
        private readonly ICarInventoryRepository _inventoryRepo;

        public CarAdminService(ICarRepository carRepo, ICarImageRepository imageRepo, ICarFeatureRepository carFeatureRepo, ICarSpecificationRepository carSpecificationRepo, ICarInventoryRepository inventoryRepo)
        {
            _carRepo = carRepo;
            _imageRepo = imageRepo;
            _carFeatureRepo = carFeatureRepo;
            _carSpecificationRepo = carSpecificationRepo;
            _inventoryRepo = inventoryRepo;
        }

        // 1. GET ALL
        public async Task<object> GetCarsAsync(
            string? search, string? brand, string? color,
            decimal? minPrice, decimal? maxPrice, CarStatus? status,
            string? transmission, string? bodyStyle,
            string? fuelType, string? location,
            bool? isDeleted, int page, int pageSize)
        {
            var result = await _carRepo.GetAdminCarsAsync(
                search, brand, color, minPrice, maxPrice, status,
                transmission, bodyStyle, fuelType, location, isDeleted, page, pageSize);

            // Dùng ngoặc nhọn { } trong Select để thoải mái viết logic tính toán
            var adminCars = result.Cars.Select(c => {

                // 1. Tính tổng số lượng xe ở tất cả các kho
                int totalQty = c.CarInventories != null ? c.CarInventories.Sum(i => i.Quantity) : 0;

                // 2. Xử lý chuỗi hiển thị Showroom (Mặc định là "Hết hàng")
                string displayLocation = "Hết hàng";

                if (totalQty > 0 && c.CarInventories != null)
                {
                    // Lọc ra các kho có xe > 0, có thông tin Showroom và Địa chỉ
                    var activeLocations = c.CarInventories
                        .Where(inv => inv.Quantity > 0 && inv.Showroom != null && !string.IsNullOrWhiteSpace(inv.Showroom.Province))
                        .Select(inv => inv.Showroom.Province) // Lấy trực tiếp cột Province luôn, quá khỏe!
                        .Distinct()
                        .ToList();

                    if (activeLocations.Any())
                    {
                        // Lấy tối đa 2 tỉnh đầu tiên để hiển thị
                        displayLocation = string.Join(", ", activeLocations.Take(2));

                        // Nếu xe phân bố ở nhiều hơn 2 tỉnh thì gắn thêm đuôi "..."
                        if (activeLocations.Count > 2)
                        {
                            displayLocation += ", ...";
                        }
                    }
                }

                // 3. Trả về Object đã được chế biến sạch sẽ cho Frontend
                return new
                {
                    c.CarId,
                    c.Name,
                    c.Brand,
                    c.Year,
                    c.Price,
                    c.ImageUrl,
                    Condition = c.Condition.ToString(),
                    Status = c.Status.ToString(),
                    c.IsDeleted,
                    TotalQuantity = totalQty,
                    Showrooms = displayLocation,
                    c.BodyStyle,
                    CreatedAt = c.CreatedAt?.ToString("dd/MM/yyyy HH:mm"),
                    UpdatedAt = c.UpdatedAt?.ToString("dd/MM/yyyy HH:mm")
                };
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

        // 2. GET DETAIL (Bản Full Options 2026 cho Admin)
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
                TotalQuantity = car.CarInventories != null ? car.CarInventories.Sum(i => i.Quantity) : 0,
                car.Transmission,
                car.BodyStyle,
                car.Description,
                car.ImageUrl,
                Condition = car.Condition.ToString(),
                Status = car.Status.ToString(),
                car.IsDeleted,
                car.CreatedAt,
                car.UpdatedAt,

                // ✅ THÊM CỤC NÀY: Quản lý tồn kho chi tiết cho Admin
                ShowroomDetails = car.CarInventories?.Select(inv => new {
                    inv.ShowroomId,
                    ShowroomName = inv.Showroom?.Name,
                    ShowroomAddress = inv.Showroom?.FullAddress, // 👈 Sửa Address thành FullAddress ở đây
                    Quantity = inv.Quantity,
                    StockStatus = inv.Quantity == 0 ? "Hết hàng" : (inv.Quantity < 3 ? "Sắp hết" : "Sẵn có")
                }).ToList(),

                // 1. NHÓM THÔNG SỐ KỸ THUẬT (Giữ nguyên logic của ní)
                Specifications = car.CarSpecifications
                    .GroupBy(s => s.Category)
                    .Select(group => new {
                        Category = group.Key,
                        Items = group.Select(i => new { i.SpecName, i.SpecValue }).ToList()
                    }).ToList(),

                // 2. DANH SÁCH TIỆN ÍCH
                Features = car.CarFeatures
                    .Select(cf => new {
                        cf.FeatureId,
                        FeatureName = cf.Feature?.FeatureName,
                        Icon = cf.Feature?.Icon
                    }).ToList(),

                // 3. GOM NHÓM ẢNH PHỤ
                GalleryImages = car.CarImages.Where(img => img.Is360Degree == false)
                    .GroupBy(img => img.ImageType)
                    .Select(group => new {
                        Category = group.Key,
                        Images = group.Select(i => new { i.CarImageId, i.Title, i.Description, i.ImageUrl }).ToList()
                    }).ToList(),

                // 4. MẢNG ẢNH 360
                Images360 = car.CarImages.Where(img => img.Is360Degree == true)
                    .Select(i => new { i.CarImageId, i.ImageUrl }).ToList()
            };
        }

        // 3. CREATE
        public async Task<(bool Success, string Message, Car? Data)> CreateCarAsync(CarCreateDto dto)
        {
            // 0. CHUẨN HÓA DỮ LIỆU ĐẦU VÀO (Xử lý vụ hoa/thường và khoảng trắng)
            if (!string.IsNullOrWhiteSpace(dto.Brand))
                dto.Brand = dto.Brand.Trim().ToUpper(); // Biến mọi kiểu nhập (vd: "  vinFast ") thành chữ IN HOA "VINFAST"

            if (!string.IsNullOrWhiteSpace(dto.Name))
                dto.Name = dto.Name.Trim(); // Dọn dẹp khoảng trắng thừa của Tên xe (vd: " VF8 " thành "VF8")

            // 1. KIỂM TRA TÊN XE
            if (await _carRepo.CheckCarListingExistAsync(dto.Name, dto.Brand, dto.Year, dto.Color, (int)dto.Condition, (decimal)(dto.Mileage ?? 0)))
                return (false, "Tin đăng này đã tồn tại rồi ní ơi!", null);

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
                Mileage = (decimal)(dto.Mileage ?? 0),   // Nhận vào đây
                Description = dto.Description, // Nhận vào đây
                Transmission = dto.Transmission,
                BodyStyle = dto.BodyStyle,

                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsDeleted = false,
                Status = dto.Status ?? CarStatus.Draft, //(Lưu nháp = 0, Nộp duyệt = 1)
                RejectionReason = null                  // dòng comment cho các sếp
            };

            // Xử lý ảnh xe
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                // 1. Tạo đường dẫn thư mục cha chứa Hãng (VD: "Cars/VINFAST")
                string subFolder = $"Cars/{car.Brand}";

                // 2. Định danh folder con cho từng xe (VD: "VINFAST_VF7")
                string targetName = $"{car.Brand}_{car.Name}";

                // 3. Gọi Shipper FileHelper đi giao hàng
                car.ImageUrl = await FileHelper.UploadFileAsync(dto.ImageFile, subFolder, targetName);
            }
            else
            {
                car.ImageUrl = "/uploads/Cars/default-car.jpg.png"; // Ảnh mặc định
            }

            // 👉 BƯỚC BẮT BUỘC: Lưu xe vào DB để EF Core tự động lấy CarId mới nhất gán vào biến 'car'
            await _carRepo.AddCarAsync(car);
            // -------------------------------------------------------------
            // TỪ ĐÂY TRỞ XUỐNG BẮT ĐẦU XỬ LÝ 2 BẢNG PHỤ DỰA VÀO CARID MỚI
            // -------------------------------------------------------------

            // 3. NHẬP KHO XE VỪA TẠO VÀO CƠ SỞ ĐÃ CHỌN
            if (dto.ShowroomId > 0)
            {
                var inventory = new CarInventory
                {
                    CarId = car.CarId,
                    ShowroomId = dto.ShowroomId,
                    Quantity = dto.Quantity,
                    DisplayStatus = "OnDisplay", // Hoặc Available tùy logic của bạn
                    UpdatedAt = DateTime.Now
                };
                await _inventoryRepo.AddInventoryAsync(inventory);
            }

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
            // 0. CHUẨN HÓA DỮ LIỆU ĐẦU VÀO (Tuyệt chiêu đồng bộ hệ thống)
            if (!string.IsNullOrWhiteSpace(dto.Brand))
                dto.Brand = dto.Brand.Trim().ToUpper(); // Ép chuẩn in hoa: "  honda " -> "HONDA"

            if (!string.IsNullOrWhiteSpace(dto.Name))
                dto.Name = dto.Name.Trim(); // Dọn dẹp khoảng trắng

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

            // 👉 Sửa lỗi CS0266: Ép kiểu int về Enum CarCondition
            car.Condition = (CarCondition)dto.Condition;

            car.FuelType = dto.FuelType;

            // 👉 Sửa lỗi CS0019: Ép kiểu về decimal và dùng 0.0 cho khớp với double
            // (Lưu ý: Nếu DTO báo lỗi không cho dùng ??,chỉ cần đổi thành: car.Mileage = (decimal)dto.Mileage; là xong)
            car.Mileage = (decimal)dto.Mileage;
            car.Transmission = dto.Transmission;
            car.BodyStyle = dto.BodyStyle;

            car.UpdatedAt = DateTime.Now;

            if (car.Status == CarStatus.Rejected)
            {
                car.Status = CarStatus.PendingApproval;
                car.RejectionReason = null; // Tẩy trắng hồ sơ
            }

            // Truyền thêm ID vào cuối cùng để loại trừ chính nó khi check trùng lặp
            if (await _carRepo.CheckCarListingExistAsync(dto.Name, dto.Brand, dto.Year, dto.Color, (int)dto.Condition, (decimal)car.Mileage, id))
            {
                return (false, "Tin đăng này bị trùng với một xe khác rồi ní ơi!", null);
            }

            try
            {
                // 3. XỬ LÝ ẢNH CHÍNH (Đồng bộ cấu trúc thư mục với lúc Create)
                if (dto.ImageFile != null)
                {
                    try
                    {
                        // A. Xác định thư mục cha và tên folder con (VD: Cars/HONDA -> HONDA_CIVIC)
                        string subFolder = $"Cars/{car.Brand}";
                        string targetName = $"{car.Brand}_{car.Name}";

                        // B. Lưu ảnh mới bằng FileHelper để tự động bóc tách thư mục
                        string newImageUrl = await FileHelper.UploadFileAsync(dto.ImageFile, subFolder, targetName);

                        // C. XÓA ẢNH CŨ (Tránh rác server)
                        if (!string.IsNullOrEmpty(car.ImageUrl) && !car.ImageUrl.Contains("default-car.jpg"))
                        {
                            // Gọi ngay ông FileHelper đi dọn rác, code sạch đẹp mướt rượt!
                            FileHelper.DeleteFile(car.ImageUrl);
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
                await _carSpecificationRepo.DeleteByCarIdAsync(id);

                if (!string.IsNullOrWhiteSpace(dto.Specifications))
                {
                    var specLines = dto.Specifications.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    var newSpecs = new List<CarSpecification>();

                    foreach (var line in specLines)
                    {
                        var parts = line.Split('|');
                        if (parts.Length == 3) // Đúng định dạng: Nhóm|Tên|Giá trị
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
                    // Bước A: Xóa sạch các tiện ích cũ
                    await _carFeatureRepo.DeleteByCarIdAsync(id);

                    // Bước B: Thêm tiện ích mới
                    if (!string.IsNullOrWhiteSpace(dto.FeatureIds))
                    {
                        var featureIds = dto.FeatureIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                       .Select(idStr => idStr.Trim())
                                                       .Where(idStr => int.TryParse(idStr, out _))
                                                       .Select(int.Parse)
                                                       .ToList();

                        var newCarFeatures = featureIds.Select(fId => new CarFeature
                        {
                            CarId = id,
                            FeatureId = fId
                        }).ToList();

                        if (newCarFeatures.Any())
                        {
                            await _carFeatureRepo.AddRangeAsync(newCarFeatures);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return (false, $"Lỗi khi cập nhật tiện ích: {ex.Message}", null);
                }

                // 6. Chốt đơn lưu xe vào Database
                await _carRepo.UpdateAsync(car);

                if (dto.ShowroomId > 0)
                {
                    // Tìm xem chiếc xe này ở cơ sở đó đã có bản ghi trong kho chưa
                    var inventory = await _inventoryRepo.GetInventoryAsync(id, dto.ShowroomId);

                    if (inventory != null)
                    {
                        // ✅ NÂNG CẤP: Cập nhật số lượng và tự động đổi trạng thái hiển thị của kho đó
                        inventory.Quantity = dto.Quantity;
                        inventory.DisplayStatus = dto.Quantity <= 0 ? "Out of stock" : "OnDisplay";
                        inventory.UpdatedAt = DateTime.Now;

                        await _inventoryRepo.UpdateInventoryAsync(inventory);
                    }
                    else if (dto.Quantity > 0) // Chỉ tạo kho mới nếu Admin nhập số lượng > 0
                    {
                        // Chữa cháy: Nếu lúc trước quên nhập kho, giờ tạo mới luôn
                        var newInventory = new CarInventory
                        {
                            CarId = id,
                            ShowroomId = dto.ShowroomId,
                            Quantity = dto.Quantity,
                            DisplayStatus = "OnDisplay",
                            UpdatedAt = DateTime.Now
                        };
                        await _inventoryRepo.AddInventoryAsync(newInventory);
                    }
                }

                await SyncCarStatusAsync(id);

                return (true, "Cập nhật xe, ảnh, thông số và kho thành công!", car);

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

                // 👉 SỬA ĐƯỜNG DẪN ẢNH PHỤ TẠI ĐÂY
                string subFolder = $"Cars/{car.Brand}";
                string targetName = $"{car.Brand}_{car.Name}";
                string imagePath = await FileHelper.UploadFileAsync(file, subFolder, targetName);

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

            // 👉 SỬA ĐƯỜNG DẪN ẢNH 360 TẠI ĐÂY
            // Gom thư mục cha và thư mục xe lại làm 1
            string subFolder = $"Cars/{car.Brand}/{car.Brand}_{car.Name}";
            // Target name là "360" để FileHelper đẻ thêm 1 thư mục con bên trong
            string targetName = "360";

            var uploadedImages = new List<CarImage>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // Truyền subFolder và targetName mới vào
                    string imagePath = await FileHelper.UploadFileAsync(file, subFolder, targetName);

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

            // 👉 GỌI FILEHELPER XÓA NHANH GỌN LẸ
            if (!string.IsNullOrEmpty(image.ImageUrl) && !image.ImageUrl.Contains("default-car.jpg") && !image.ImageUrl.StartsWith("http"))
            {
                FileHelper.DeleteFile(image.ImageUrl); // Dùng đúng 1 dòng "đồ nhà trồng được"
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

            // A. XÓA FOLDER VẬT LÝ (Đã nâng cấp theo chuẩn Cars/Hãng/Hãng_TênXe)
            // 1. Dựng lại đúng tên Hãng và Tên xe như lúc Create
            string brandFolder = car.Brand?.Trim().ToUpper() ?? "UNKNOWN";
            string targetName = $"{brandFolder}_{car.Name?.Trim()}";

            // 2. Dọn dẹp ký tự đặc biệt y chang cách FileHelper đã làm để tìm trúng phóc thư mục
            string cleanTargetName = string.Join("_", targetName.Split(Path.GetInvalidFileNameChars())).Replace(" ", "_");

            // 3. Ghép đường dẫn: wwwroot/uploads/Cars/VINFAST/VINFAST_VF7
            var carFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "Cars", brandFolder, cleanTargetName);

            // 4. Quét sạch sành sanh
            if (Directory.Exists(carFolderPath))
            {
                // Tham số 'true' ở đây cực kỳ lợi hại: Nó sẽ xóa cái folder này và toàn bộ ảnh chính, ảnh phụ, lẫn folder 360 bên trong!
                Directory.Delete(carFolderPath, true);
            }
            else if (!string.IsNullOrEmpty(car.ImageUrl) && !car.ImageUrl.Contains("default-car.jpg"))
            {
                // Chữa cháy: Nếu xui xui thư mục không còn mà ảnh chính vẫn kẹt ở ngoài thì gọi FileHelper xử lý
                FileHelper.DeleteFile(car.ImageUrl);
            }

            // B. XÓA DỮ LIỆU TRONG CÁC BẢNG PHỤ TRƯỚC (Chuẩn đét!)
            await _imageRepo.DeleteAllImagesByCarIdAsync(id);
            await _carFeatureRepo.DeleteByCarIdAsync(id);
            await _carSpecificationRepo.DeleteByCarIdAsync(id);

            // C. CUỐI CÙNG MỚI XÓA XE VÀO HƯ VÔ
            await _carRepo.DeleteCarAsync(id);

            return true;
        }

        // Viết một hàm private nhỏ gọn trong CarAdminService.cs
        private async Task SyncCarStatusAsync(int carId)
        {
            // 1. Tính tổng tồn kho hiện tại
            int totalQuantity = await _inventoryRepo.GetTotalQuantityByCarIdAsync(carId);

            // 2. Lấy chiếc xe lên
            var car = await _carRepo.GetByIdAsync(carId);
            if (car == null) return;

            // 3. Logic chuyển đổi thông minh
            if (totalQuantity <= 0 && car.Status == CarStatus.Available)
            {
                // Đang bán mà kho tụt xuống 0 -> Tự động chuyển thành Hết hàng
                car.Status = CarStatus.Out_of_stock;
                await _carRepo.UpdateAsync(car);
            }
            else if (totalQuantity > 0 && car.Status == CarStatus.Out_of_stock)
            {
                // Đang hết hàng mà kho lại lớn hơn 0 (do nhập thêm) -> Tự động mở bán lại
                car.Status = CarStatus.Available;
                await _carRepo.UpdateAsync(car);
            }
            // Nếu xe đang là Draft hoặc Coming_Soon thì kệ nó, không tự động đổi.
        }

        // ==========================================
        // KHU VỰC QUYỀN LỰC CỦA MANAGER (QUẢN LÝ)
        // ==========================================

        // 11. DUYỆT XE (Cho phép xe lên Web)
        public async Task<(bool Success, string Message)> ApproveCarAsync(int carId)
        {
            var car = await _carRepo.GetByIdAsync(carId);
            if (car == null) return (false, "Không tìm thấy xe!");

            if (car.Status == CarStatus.Available)
                return (false, "Xe này đang bán rồi, duyệt gì nữa ní!");

            car.Status = CarStatus.Available; // Đóng mộc ĐÃ DUYỆT!
            car.RejectionReason = null; // Chắc chắn là không còn lý do chê bai gì nữa
            car.UpdatedAt = DateTime.Now;

            await _carRepo.UpdateAsync(car);
            return (true, "Đã duyệt xe thành công! Xe đã lên sóng.");
        }

        // 12. TỪ CHỐI XE (Kèm lời dặn dò cho lính)
        public async Task<(bool Success, string Message)> RejectCarAsync(int carId, string reason)
        {
            var car = await _carRepo.GetByIdAsync(carId);
            if (car == null) return (false, "Không tìm thấy xe!");

            if (string.IsNullOrWhiteSpace(reason))
                return (false, "Từ chối thì phải ghi rõ lý do cho lính nó biết đường sửa chứ sếp!");

            car.Status = CarStatus.Rejected; // Đóng mộc TỪ CHỐI!
            car.RejectionReason = reason; // Lưu lời vàng ý ngọc của Sếp vào DB
            car.UpdatedAt = DateTime.Now;

            await _carRepo.UpdateAsync(car);
            return (true, "Đã từ chối và gửi phản hồi lại cho nhân viên!");
        }
    }
}