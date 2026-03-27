using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Linq;
using LogicBusiness.Interfaces.Shared;

namespace LogicBusiness.Services.Admin
{
    public class CarAdminService : ICarAdminService
    {
        private readonly ICarRepository _carRepo;
        private readonly ICarImageRepository _imageRepo;
        private readonly ICarFeatureRepository _carFeatureRepo;             
        private readonly ICarSpecificationRepository _carSpecificationRepo;
        private readonly ICarInventoryRepository _inventoryRepo;
        private readonly INotificationService _notiService;

        public CarAdminService(ICarRepository carRepo, ICarImageRepository imageRepo, ICarFeatureRepository carFeatureRepo, ICarSpecificationRepository carSpecificationRepo, ICarInventoryRepository inventoryRepo, INotificationService notiService)
        {
            _carRepo = carRepo;
            _imageRepo = imageRepo;
            _carFeatureRepo = carFeatureRepo;
            _carSpecificationRepo = carSpecificationRepo;
            _inventoryRepo = inventoryRepo;
            _notiService = notiService;
        }

        // 1. GET ALL
        public async Task<object> GetCarsAsync(
            string? search, string? brand, string? color,
            decimal? minPrice, decimal? maxPrice, CarStatus? status,
            string? transmission, string? bodyStyle,
            string? fuelType, string? location,
            bool? isDeleted, int page, int pageSize,
            int? userShowroomId = null) // 👈 Đã thêm vũ khí Phân quyền (Nhớ thêm vào Interface nha)
        {
            // Nhớ truyền userShowroomId xuống Repo để nó lọc xe theo Showroom cho Manager
            var result = await _carRepo.GetAdminCarsAsync(
                search, brand, color, minPrice, maxPrice, status,
                transmission, bodyStyle, fuelType, location, isDeleted, page, pageSize, userShowroomId);

            var adminCars = result.Cars.Select(c => {

                // 1. Tính tổng số lượng
                int totalQty = c.CarInventories != null ? c.CarInventories.Sum(i => i.Quantity) : 0;
                string displayLocation = "";

                // 👇 ĐỒNG BỘ LOGIC Y CHANG BÊN CUSTOMER 👇

                // ƯU TIÊN 1: Check trạng thái "Sắp về"
                if (c.Status == CarStatus.COMING_SOON) // Chỗ này ní nhớ gõ đúng tên Enum trong hệ thống của ní nha
                {
                    displayLocation = "Sắp về";
                }
                // ƯU TIÊN 2: Nếu xe đang bán nhưng hết tồn kho
                else if (totalQty == 0)
                {
                    displayLocation = "Hết hàng";
                }
                // ƯU TIÊN 3: Có hàng trong kho
                else if (c.CarInventories != null)
                {
                    var activeLocations = c.CarInventories
                        .Where(inv => inv.Quantity > 0 && inv.Showroom != null && !string.IsNullOrWhiteSpace(inv.Showroom.Province))
                        .Select(inv => inv.Showroom.Province)
                        .Distinct()
                        .ToList();

                    if (activeLocations.Any())
                    {
                        displayLocation = string.Join(", ", activeLocations.Take(2));
                        if (activeLocations.Count > 2)
                        {
                            displayLocation += ", ...";
                        }
                    }
                    else
                    {
                        displayLocation = "Đang cập nhật vị trí"; // Lỡ kho có xe mà quên gắn Showroom
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

        // 2. GET DETAIL (Bản Nâng Cấp 2026: Phân quyền & Hỗ trợ dải phim 360)
        public async Task<object?> GetCarDetailAsync(int id, string userRole, int? userShowroomId)
        {
            var car = await _carRepo.GetCarDetailForAdminAsync(id);
            if (car == null) return null;

            // 1. 🛡️ PHÂN QUYỀN TỒN KHO: Lọc ngay từ vòng gửi xe
            var allowedInventories = car.CarInventories;
            if (userRole != "Admin" && userShowroomId.HasValue)
            {
                allowedInventories = car.CarInventories?.Where(inv => inv.ShowroomId == userShowroomId.Value).ToList();
            }

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
                TotalQuantity = allowedInventories != null ? allowedInventories.Sum(i => i.Quantity) : 0,
                car.Transmission,
                car.BodyStyle,
                car.Description,
                car.ImageUrl,
                Condition = car.Condition.ToString(),
                Status = car.Status.ToString(),
                car.IsDeleted,
                car.CreatedAt,
                car.UpdatedAt,

                // ✅ Tồn kho chi tiết đã lọc
                ShowroomDetails = allowedInventories?.Select(inv => new {
                    inv.ShowroomId,
                    ShowroomName = inv.Showroom?.Name,
                    ShowroomAddress = inv.Showroom?.FullAddress,
                    Quantity = inv.Quantity,
                    StockStatus = inv.Quantity == 0 ? "Hết hàng" : (inv.Quantity < 3 ? "Sắp hết" : "Sẵn có")
                }).ToList(),

                // 2. 🎞️ DẢI PHIM 360 ĐỘ (Sắp xếp chuẩn để FE làm Slider/Popup)
                // Nếu mảng này rỗng ([]), FE sẽ tự hiện thông báo "Chưa có view 360"
                Images360 = car.CarImages
                    .Where(img => img.Is360Degree == true)
                    .OrderBy(img => img.Title) // 👈 Quan trọng: Sắp xếp theo số thứ tự dải phim (1, 2, ..., 36)
                    .Select(i => new {
                        i.CarImageId,
                        i.ImageUrl,
                        FrameOrder = i.Title // 👈 Trả về thứ tự để FE biết đặt vào ô nào trong dải phim
                    }).ToList(),

                // 3. NHÓM THÔNG SỐ KỸ THUẬT
                Specifications = car.CarSpecifications
                    .GroupBy(s => s.Category)
                    .Select(group => new {
                        Category = group.Key,
                        Items = group.Select(i => new { i.SpecName, i.SpecValue }).ToList()
                    }).ToList(),

                // 4. DANH SÁCH TIỆN ÍCH
                Features = car.CarFeatures
                    .Select(cf => new {
                        cf.FeatureId,
                        FeatureName = cf.Feature?.FeatureName,
                        Icon = cf.Feature?.Icon
                    }).ToList(),

                // 5. GOM NHÓM ẢNH PHỤ (Gallery)
                GalleryImages = car.CarImages.Where(img => img.Is360Degree == false)
                    .GroupBy(img => img.ImageType)
                    .Select(group => new {
                        Category = group.Key,
                        Images = group.Select(i => new { i.CarImageId, i.Title, i.Description, i.ImageUrl }).ToList()
                    }).ToList()
            };
        }

        // 3. CREATE
        public async Task<(bool Success, string Message, Car? Data)> CreateCarAsync(CarCreateDto dto, string userRole, int? userShowroomId)
        {
            // 0. CHUẨN HÓA DỮ LIỆU
            if (!string.IsNullOrWhiteSpace(dto.Brand)) dto.Brand = dto.Brand.Trim().ToUpper();
            if (!string.IsNullOrWhiteSpace(dto.Name)) dto.Name = dto.Name.Trim();

            // A. Xác định Showroom mục tiêu: Manager/Sales thì ép về nhà mình, Admin thì tùy chọn
            int targetShowroomId = (userRole != "Admin" && userShowroomId.HasValue) ? userShowroomId.Value : dto.ShowroomId;

            // B. Xác định Trạng thái: Sếp tạo thì lên sóng luôn, lính tạo thì chờ duyệt
            CarStatus finalStatus = (userRole == "Admin" || userRole == "ShowroomManager")
                                    ? (dto.Status ?? CarStatus.Available)
                                    : CarStatus.PendingApproval;

            // --- 👇 BẮT ĐẦU PHÙ PHÉP PHÂN LUỒNG 👇 ---

            // 1. LUỒNG XE MỚI: Kiểm tra xem mẫu xe này đã tồn tại trong "Danh mục hệ thống" chưa
            if (dto.Condition == CarCondition.New)
            {
                // Ní cần viết thêm hàm GetExistingNewCarAsync bên Repo nhé (Check theo Tên + Hãng + Năm + Condition=New)
                var existingCar = await _carRepo.GetExistingNewCarAsync(dto.Name, dto.Brand, dto.Year);

                if (existingCar != null)
                {
                    // THẤY RỒI! Không tạo xe mới, chỉ cập nhật số lượng vào kho chi nhánh
                    var inventory = await _inventoryRepo.GetInventoryAsync(existingCar.CarId, targetShowroomId);
                    if (inventory != null)
                    {
                        inventory.Quantity += dto.Quantity;
                        inventory.UpdatedAt = DateTime.Now;
                        await _inventoryRepo.UpdateInventoryAsync(inventory);
                    }
                    else
                    {
                        await _inventoryRepo.AddInventoryAsync(new CarInventory
                        {
                            CarId = existingCar.CarId,
                            ShowroomId = targetShowroomId,
                            Quantity = dto.Quantity,
                            DisplayStatus = "OnDisplay",
                            UpdatedAt = DateTime.Now
                        });
                    }
                    return (true, $"Mẫu '{existingCar.Name}' đã có trên hệ thống. Tui đã tự động cộng {dto.Quantity} xe vào kho chi nhánh ní!", existingCar);
                }
            }

            // 2. LUỒNG XE CŨ HOẶC XE MỚI CHƯA CÓ MẪU: Tiến hành tạo bản ghi xe mới
            // Kiểm tra trùng lặp tin đăng (để tránh lính bấm nhầm 2 lần)
            if (await _carRepo.CheckCarListingExistAsync(dto.Name, dto.Brand, dto.Year, dto.Color, (int)dto.Condition, (decimal)(dto.Mileage ?? 0)))
                return (false, "Tin đăng này y hệt một cái khác đã có, ní kiểm tra lại coi!", null);

            var car = new Car
            {
                Name = dto.Name,
                Brand = dto.Brand,
                Year = dto.Year,
                Model = dto.Model,
                Color = dto.Color,
                Condition = dto.Condition,
                Price = dto.Price,
                FuelType = dto.FuelType,
                Mileage = (decimal)(dto.Mileage ?? 0),
                Description = dto.Description,
                Transmission = dto.Transmission,
                BodyStyle = dto.BodyStyle,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Status = finalStatus,
                IsDeleted = false
            };

            // Xử lý ảnh (Thư mục theo hãng)
            if (dto.ImageFile != null)
            {
                string subFolder = $"Cars/{car.Brand}";
                string targetName = $"{car.Brand}_{car.Name}";
                car.ImageUrl = await FileHelper.UploadFileAsync(dto.ImageFile, subFolder, targetName);
            }
            else car.ImageUrl = "/uploads/Cars/default-car.jpg.png";

            await _carRepo.AddCarAsync(car);

            // 3. NHẬP KHO CHI NHÁNH
            if (targetShowroomId > 0)
            {
                await _inventoryRepo.AddInventoryAsync(new CarInventory
                {
                    CarId = car.CarId,
                    ShowroomId = targetShowroomId,
                    Quantity = dto.Quantity,
                    DisplayStatus = (finalStatus == CarStatus.Available) ? "OnDisplay" : "Pending",
                    UpdatedAt = DateTime.Now
                });
            }

            // 4. XỬ LÝ FEATURES & SPECS (Đoạn này ní giữ nguyên logic cũ của ní)
            if (!string.IsNullOrWhiteSpace(dto.FeatureIds))
            {
                var fIds = dto.FeatureIds.Split(',').Select(id => id.Trim()).Where(id => int.TryParse(id, out _)).Select(int.Parse).ToList();
                var carFeatures = fIds.Select(fId => new CarFeature { CarId = car.CarId, FeatureId = fId });
                await _carFeatureRepo.AddRangeAsync(carFeatures);
            }

            if (!string.IsNullOrWhiteSpace(dto.Specifications))
            {
                var specLines = dto.Specifications.Split(';', StringSplitOptions.RemoveEmptyEntries);
                var carSpecs = specLines.Select(line => line.Split('|')).Where(p => p.Length == 3)
                    .Select(p => new CarSpecification { CarId = car.CarId, Category = p[0].Trim(), SpecName = p[1].Trim(), SpecValue = p[2].Trim() }).ToList();
                if (carSpecs.Any()) await _carSpecificationRepo.AddRangeAsync(carSpecs);
            }

            if (finalStatus == CarStatus.PendingApproval)
            {
                await _notiService.CreateNotificationAsync(
                    userId: null,
                    showroomId: targetShowroomId, // Báo cho mấy ông sếp cùng chi nhánh
                    title: "Có xe mới cần duyệt",
                    content: $"Nhân viên vừa đăng mẫu {car.Brand} {car.Name}. Sếp vào duyệt nhé!",
                    actionUrl: $"/admin/cars/approve/{car.CarId}",
                    type: "CarApproval"
                );
            }

            string finalMsg = (finalStatus == CarStatus.Available) ? "Đã lên sàn con xe mới tinh!" : "Đã tạo yêu cầu, đợi sếp gật đầu là xe lên sóng nha!";
            return (true, finalMsg, car);
        }


        // 4. UPDATE (Bản Chốt Hạ 2026)
        public async Task<(bool Success, string Message, Car? Car)> UpdateCarAsync(int id, CarUpdateDto dto, string userRole, int? userShowroomId)
        {
            // 1. Tìm xe duy nhất 1 lần ở đầu hàm
            var car = await _carRepo.GetByIdAsync(id);
            if (car == null) return (false, "Không tìm thấy xe!", null);

            // 2. 🛡️ BẢO VỆ DỮ LIỆU: Manager/Sales không được sửa xe chi nhánh khác
            if (userRole != "Admin" && userShowroomId.HasValue)
            {
                var inventories = await _inventoryRepo.GetInventoriesByCarIdAsync(id);
                if (!inventories.Any(i => i.ShowroomId == userShowroomId.Value))
                {
                    return (false, "Sếp ơi, xe này không thuộc quyền quản lý của chi nhánh mình!", null);
                }
            }

            // 3. Chuẩn hóa dữ liệu
            string cleanBrand = dto.Brand?.Trim().ToUpper() ?? "";
            string cleanName = dto.Name?.Trim() ?? "";

            // 👇 4. LOGIC CHECK TRÙNG MẪU HỆ THỐNG (QUAN TRỌNG NHẤT) 👇
            if ((CarCondition)dto.Condition == CarCondition.New)
            {
                // Kiểm tra xem cái "Combo" mới này đã có con xe nào khác đang dùng chưa
                var duplicateModel = await _carRepo.GetExistingNewCarAsync(cleanName, cleanBrand, dto.Year);

                // Nếu tìm thấy một con xe khác (ID khác) mà trùng y chang tên/hãng/năm
                if (duplicateModel != null && duplicateModel.CarId != id)
                {
                    return (false, $"Ní ơi, mẫu '{cleanName}' đời {dto.Year} đã có trên hệ thống rồi. Không được sửa con này trùng với mẫu đó, sẽ bị rác dữ liệu!", null);
                }
            }

            // 5. Cập nhật thông tin cơ bản
            car.Name = cleanName;
            car.Brand = cleanBrand;
            car.Model = dto.Model;
            car.Price = (decimal)dto.Price;
            car.Year = dto.Year;
            car.Description = dto.Description;
            car.Condition = (CarCondition)dto.Condition;
            car.FuelType = dto.FuelType;
            car.Mileage = (decimal)dto.Mileage;
            car.Transmission = dto.Transmission;
            car.BodyStyle = dto.BodyStyle;
            car.UpdatedAt = DateTime.Now;

            // 6.LOGIC TRẠNG THÁI THÔNG MINH (Lưu nháp vs Gửi duyệt)
            if (userRole == "Admin" || userRole == "ShowroomManager")
            {
                // Sếp sửa thì cho phép sếp quyết định trạng thái luôn (nếu sếp có gửi Status lên)
                if (dto.Status.HasValue) car.Status = dto.Status.Value;
            }
            else if (userRole == "ShowroomSales" || userRole == "Staff")
            {
                // Nhân viên: Nếu bấm "Nộp bài" (Status = PendingApproval) thì mới gửi sếp
                // Còn lại (Lưu nháp hoặc không chọn) thì cứ để là Draft cho lính sửa tiếp
                if (dto.Status == CarStatus.PendingApproval)
                {
                    car.Status = CarStatus.PendingApproval;
                    car.RejectionReason = null; // Gửi lại là xóa lý do cũ ngay cho sạch sẽ
                }
                else
                {
                    car.Status = CarStatus.Draft;
                }
            }

            // 7. Kiểm tra trùng lặp tin đăng chung (Dành cho xe cũ - check thêm màu sắc, ODO...)
            if (await _carRepo.CheckCarListingExistAsync(car.Name, car.Brand, car.Year, dto.Color, (int)car.Condition, (decimal)car.Mileage, id))
            {
                return (false, "Ní sửa gì mà nó trùng khít với một tin đăng khác vậy? Kiểm tra lại ODO hoặc màu sắc coi!", null);
            }

            try
            {
                // 8. XỬ LÝ ẢNH CHÍNH
                if (dto.ImageFile != null)
                {
                    string subFolder = $"Cars/{car.Brand}";
                    string targetName = $"{car.Brand}_{car.Name}";
                    string newImageUrl = await FileHelper.UploadFileAsync(dto.ImageFile, subFolder, targetName);

                    if (!string.IsNullOrEmpty(car.ImageUrl) && !car.ImageUrl.Contains("default-car.jpg"))
                    {
                        FileHelper.DeleteFile(car.ImageUrl);
                    }
                    car.ImageUrl = newImageUrl;
                }

                // 9. XỬ LÝ THÔNG SỐ KỸ THUẬT & TIỆN ÍCH (Giữ nguyên logic Xóa-Xây của ní)
                await _carSpecificationRepo.DeleteByCarIdAsync(id);
                if (!string.IsNullOrWhiteSpace(dto.Specifications))
                {
                    var specLines = dto.Specifications.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    var newSpecs = specLines.Select(line => line.Split('|')).Where(p => p.Length == 3)
                        .Select(p => new CarSpecification { CarId = id, Category = p[0].Trim(), SpecName = p[1].Trim(), SpecValue = p[2].Trim() }).ToList();
                    if (newSpecs.Any()) await _carSpecificationRepo.AddRangeAsync(newSpecs);
                }

                await _carFeatureRepo.DeleteByCarIdAsync(id);
                if (!string.IsNullOrWhiteSpace(dto.FeatureIds))
                {
                    var featureIds = dto.FeatureIds.Split(',').Select(s => s.Trim()).Where(s => int.TryParse(s, out _)).Select(int.Parse).ToList();
                    var newCarFeatures = featureIds.Select(fId => new CarFeature { CarId = id, FeatureId = fId }).ToList();
                    await _carFeatureRepo.AddRangeAsync(newCarFeatures);
                }

                // 10. CHỐT ĐƠN LƯU DATABASE
                await _carRepo.UpdateAsync(car);

                // 11. CẬP NHẬT KHO CHI NHÁNH
                if (dto.ShowroomId > 0)
                {
                    var inventory = await _inventoryRepo.GetInventoryAsync(id, dto.ShowroomId);
                    if (inventory != null)
                    {
                        inventory.Quantity = dto.Quantity;
                        inventory.DisplayStatus = dto.Quantity <= 0 ? "Out of stock" : "OnDisplay";
                        inventory.UpdatedAt = DateTime.Now;
                        await _inventoryRepo.UpdateInventoryAsync(inventory);
                    }
                    else if (dto.Quantity > 0)
                    {
                        await _inventoryRepo.AddInventoryAsync(new CarInventory
                        {
                            CarId = id,
                            ShowroomId = dto.ShowroomId,
                            Quantity = dto.Quantity,
                            DisplayStatus = "OnDisplay",
                            UpdatedAt = DateTime.Now
                        });
                    }
                }

                if ((userRole == "ShowroomSales" || userRole == "Staff") && car.Status == CarStatus.PendingApproval)
                {
                    await _notiService.CreateNotificationAsync(
                        userId: null,
                        showroomId: dto.ShowroomId, // Báo cho sếp của chi nhánh này
                        title: "Có bản cập nhật xe cần duyệt 📝",
                        content: $"Nhân viên vừa sửa và nộp lại thông tin mẫu {car.Brand} {car.Name}. Sếp vào kiểm tra nhé!",
                        actionUrl: $"/admin/cars/approve/{car.CarId}", // Trỏ sếp thẳng vào trang duyệt
                        type: "CarApproval"
                    );
                }

                await SyncCarStatusAsync(id);
                return (true, "Cập nhật xe và quản lý mẫu hệ thống thành công!", car);
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

        // 6. UPLOAD 360 (Bản Chốt Hạ: Tôn trọng tuyệt đối thứ tự FE kéo thả)
        public async Task<(bool Success, string Message)> Upload360ImagesAsync(int carId, List<IFormFile> files)
        {
            var car = await _carRepo.GetCarByIdAsync(carId);
            if (car == null) return (false, "Xe không tồn tại.");

            // Xử lý trường hợp: Nhân viên bấm xóa toàn bộ 360 (FE gửi mảng rỗng)
            if (files == null || files.Count == 0)
            {
                await _imageRepo.DeleteAll360ImagesByCarIdAsync(carId);
                return (true, "Đã xóa sạch dải phim 360 của xe này.");
            }

            string subFolder = $"Cars/{car.Brand}/{car.Brand}_{car.Name}/360";

            // Xóa sạch ảnh 360 cũ để Up bộ mới cho gọn gàng
            await _imageRepo.DeleteAll360ImagesByCarIdAsync(carId);

            // 👇 BÍ KÍP Ở ĐÂY: KHÔNG DÙNG OrderBy() NỮA!
            // FE gửi mảng 'files' xuống theo thứ tự nào (do nhân viên kéo thả), 
            // mình lưu y xì thứ tự đó vào vòng lặp luôn.
            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (file.Length > 0)
                {
                    // Mẹo nhỏ: Thêm 1 đoạn mã thời gian (Tick) vào đuôi tên file.
                    // Tránh tình trạng trình duyệt bị Cache ảnh cũ khi nhân viên up lại bộ mới.
                    string targetName = $"frame_{i + 1}_{DateTimeOffset.Now.ToUnixTimeSeconds()}";

                    string imagePath = await FileHelper.UploadFileAsync(file, subFolder, targetName);

                    var carImage = new CarImage
                    {
                        CarId = carId,
                        ImageUrl = imagePath,
                        Is360Degree = true,
                        Title = (i + 1).ToString(), // 👈 Đánh số thứ tự Frame (1, 2, 3...) theo đúng Index của FE
                        CreatedAt = DateTime.Now
                    };
                    await _imageRepo.AddCarImageAsync(carImage);
                }
            }

            return (true, $"Tải lên thành công dải phim {files.Count} chưởng!");
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
        public async Task<bool> HardDeleteCarAsync(int id, string userRole)
        {
            if (userRole != "Admin") return false;

            // 1. Lấy Full thông tin xe (dùng GetCarDetailForAdminAsync để lôi cả list ảnh phụ lên)
            var car = await _carRepo.GetCarDetailForAdminAsync(id);
            if (car == null) return false;

            // Chỉ cho phép xóa cứng khi xe đang Nháp hoặc Đã bị Soft Delete
            if (car.IsDeleted != true && car.Status != CarStatus.Draft) return false;

            // ==========================================================
            // --- A. CHIẾN DỊCH QUÉT RÁC Ổ CỨNG (FILE VẬT LÝ) ---
            // ==========================================================
            var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            // Bước 1: Xóa chính xác từng tấm ảnh phụ và ảnh 360 dựa trên URL trong Database
            if (car.CarImages != null && car.CarImages.Any())
            {
                foreach (var img in car.CarImages)
                {
                    if (!string.IsNullOrEmpty(img.ImageUrl) && !img.ImageUrl.StartsWith("http"))
                    {
                        var filePath = Path.Combine(wwwrootPath, img.ImageUrl.TrimStart('/'));
                        if (File.Exists(filePath)) File.Delete(filePath);
                    }
                }
            }

            // Bước 2: Xóa ảnh đại diện chính của xe (Main Image)
            if (!string.IsNullOrEmpty(car.ImageUrl) && !car.ImageUrl.Contains("default-car.jpg") && !car.ImageUrl.StartsWith("http"))
            {
                var mainImagePath = Path.Combine(wwwrootPath, car.ImageUrl.TrimStart('/'));
                if (File.Exists(mainImagePath)) File.Delete(mainImagePath);
            }

            // Bước 3: Cố gắng xóa luôn cái thư mục rỗng của xe đó cho sạch sẽ 
            try
            {
                string brandFolder = car.Brand?.Trim().ToUpper() ?? "UNKNOWN";
                string targetName = $"{brandFolder}_{car.Name?.Trim()}";
                string cleanTargetName = string.Join("_", targetName.Split(Path.GetInvalidFileNameChars())).Replace(" ", "_");
                var carFolderPath = Path.Combine(wwwrootPath, "uploads", "Cars", brandFolder, cleanTargetName);

                if (Directory.Exists(carFolderPath))
                {
                    Directory.Delete(carFolderPath, true); // Quét nốt cái vỏ folder
                }
            }
            catch { /* Lỡ thư mục đã mất từ trước thì kệ nó, ảnh file vật lý đã xóa sạch ở trên rồi */ }

            // ==========================================================
            // --- B. CHIẾN DỊCH QUÉT DATABASE (Dọn rễ trước khi đốn cây) ---
            // ==========================================================

            // 1. Dọn ảnh trong DB
            await _imageRepo.DeleteAllImagesByCarIdAsync(id);

            // 2. Dọn tiện ích (Features)
            await _carFeatureRepo.DeleteByCarIdAsync(id);

            // 3. Dọn thông số kỹ thuật (Specifications)
            await _carSpecificationRepo.DeleteByCarIdAsync(id);

            // 4. Dọn kho bãi (CỰC KỲ QUAN TRỌNG ĐỂ KHÔNG LỖI KHÓA NGOẠI)
            // 👉 Lưu ý: Ní nhớ tạo hàm này bên ICarInventoryRepository nhé!
            await _inventoryRepo.DeleteInventoriesByCarIdAsync(id);

            // ==========================================================
            // --- C. CÚ CHỐT HẠ: ĐƯA THẰNG CHA VÀO HƯ VÔ ---
            // ==========================================================
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


        // NHÂN BẢN XE
        public async Task<(bool Success, string Message, int? NewCarId)> CloneCarAsync(int id, string userRole, int? userShowroomId)
        {
            // 1. Lấy "hồn" con xe cũ lên (đảm bảo hàm này trong Repo đã Include Specs/Features)
            var oldCar = await _carRepo.GetCarDetailForAdminAsync(id);
            if (oldCar == null) return (false, "Không tìm thấy con xe gốc để nhân bản!", null);

            // 2. 🛡️ Bảo vệ: Nhân viên chi nhánh nào chỉ được nhân bản xe chi nhánh đó
            if (userRole != "Admin" && userShowroomId.HasValue)
            {
                var inventories = await _inventoryRepo.GetInventoriesByCarIdAsync(id);
                if (!inventories.Any(i => i.ShowroomId == userShowroomId.Value))
                    return (false, "Ní không được phép nhân bản xe của showroom chi nhánh khác đâu nhé!", null);
            }

            // 3. Tạo "xác" xe mới (Copy thông tin, Reset trạng thái)
            var newCar = new Car
            {
                Name = oldCar.Name + " (Bản sao chưa duyệt)", // Hậu tố nhắc nhở
                Brand = oldCar.Brand,
                Model = oldCar.Model,
                Year = oldCar.Year,
                Price = oldCar.Price,
                Color = oldCar.Color,
                Mileage = oldCar.Mileage,
                FuelType = oldCar.FuelType,
                Transmission = oldCar.Transmission,
                BodyStyle = oldCar.BodyStyle,
                Description = oldCar.Description,
                Condition = oldCar.Condition,

                // 👇 QUAN TRỌNG: Ép về Nháp và xóa ảnh cũ để bắt buộc lính phải vào sửa
                Status = CarStatus.Draft,
                ImageUrl = "/uploads/Cars/default-car.jpg.png",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsDeleted = false
            };

            await _carRepo.AddCarAsync(newCar);

            // 4. Nhân bản thông số kỹ thuật (Specifications)
            if (oldCar.CarSpecifications != null && oldCar.CarSpecifications.Any())
            {
                var newSpecs = oldCar.CarSpecifications.Select(s => new CarSpecification
                {
                    CarId = newCar.CarId,
                    Category = s.Category,
                    SpecName = s.SpecName,
                    SpecValue = s.SpecValue
                }).ToList();
                await _carSpecificationRepo.AddRangeAsync(newSpecs);
            }

            // 5. Nhân bản tiện ích (Features)
            if (oldCar.CarFeatures != null && oldCar.CarFeatures.Any())
            {
                var newFeatures = oldCar.CarFeatures.Select(f => new CarFeature
                {
                    CarId = newCar.CarId,
                    FeatureId = f.FeatureId
                }).ToList();
                await _carFeatureRepo.AddRangeAsync(newFeatures);
            }

            // 6. Gắn vào kho của showroom đang thực hiện nhân bản
            int targetShowroomId = (userRole != "Admin" && userShowroomId.HasValue) ? userShowroomId.Value : 1; // Mặc định về 1 nếu là Admin chưa chọn

            await _inventoryRepo.AddInventoryAsync(new CarInventory
            {
                CarId = newCar.CarId,
                ShowroomId = targetShowroomId,
                Quantity = 1, // Xe cũ clone ra mặc định số lượng là 1
                DisplayStatus = "Pending", // Kho cũng để trạng thái chờ
                UpdatedAt = DateTime.Now
            });

            return (true, "Đã nhân bản bản nháp thành công! Ní hãy vào cập nhật ảnh thực tế và bấm 'Lưu' để gửi sếp duyệt nhé.", newCar.CarId);
        }

        // TÌM KIẾM MẪU XE MỚI (Dành cho UI chọn mẫu có sẵn)
        public async Task<IEnumerable<object>> SearchMasterModelsAsync(string query)
        {
            var cars = await _carRepo.SearchMasterCarsAsync(query); // Ní viết hàm Search này bên Repo nhé
            return cars.Select(c => new {
                c.CarId,
                c.Name,
                c.Brand,
                c.Year,
                c.ImageUrl,
                DisplayTitle = $"{c.Brand} {c.Name} ({c.Year})"
            });
        }

        // ==========================================
        // KHU VỰC QUYỀN LỰC CỦA MANAGER (QUẢN LÝ)
        // ==========================================

        // 11. DUYỆT XE (Có phân quyền Manager)
        public async Task<(bool Success, string Message)> ApproveCarAsync(int carId, string userRole, int? userShowroomId) // 👈 Thêm 2 tham số
        {
            var car = await _carRepo.GetByIdAsync(carId);
            if (car == null) return (false, "Không tìm thấy xe!");

            if (car.Status == CarStatus.Available)
                return (false, "Xe này đang bán rồi, duyệt gì nữa ní!");

            // 👇 BÍ KÍP: CHẶN QUẢN LÝ DUYỆT LÁO XE CHI NHÁNH KHÁC
            if (userRole == "ShowroomManager" && userShowroomId.HasValue)
            {
                var inventories = await _inventoryRepo.GetInventoriesByCarIdAsync(carId);
                // Nếu con xe này KHÔNG nằm trong Showroom của Quản lý đó -> Chửi!
                if (!inventories.Any(i => i.ShowroomId == userShowroomId.Value))
                {
                    return (false, "Sếp ơi, xe này thuộc chi nhánh khác, sếp không có thẩm quyền duyệt đâu!");
                }
            }

            car.Status = CarStatus.Available; // Đóng mộc ĐÃ DUYỆT!
            car.RejectionReason = null;
            car.UpdatedAt = DateTime.Now;

            await _carRepo.UpdateAsync(car);

            var showroomId = (await _inventoryRepo.GetInventoriesByCarIdAsync(carId)).FirstOrDefault()?.ShowroomId;
            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: showroomId, // Báo cho cả lò biết xe đã lên sàn
                title: "Xe đã lên sóng! 🎉",
                content: $"Sếp đã duyệt mẫu {car.Brand} {car.Name}. Anh em đẩy số đi!",
                actionUrl: $"/admin/cars/detail/{carId}",
                type: "CarApproval"
            );
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

            var showroomId = (await _inventoryRepo.GetInventoriesByCarIdAsync(carId)).FirstOrDefault()?.ShowroomId;
            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: showroomId,
                title: "Xe bị từ chối duyệt 🚨",
                content: $"Mẫu {car.Brand} {car.Name} bị sếp chê. Lý do: {reason}",
                actionUrl: $"/admin/cars/edit/{carId}", // Trỏ link về chỗ sửa xe luôn
                type: "CarApproval"
            );
            return (true, "Đã từ chối và gửi phản hồi lại cho nhân viên!");

        }

        // 13. ĐỔI TRẠNG THÁI NHANH (Bàn tay Phật tổ của Sếp)
        public async Task<(bool Success, string Message)> ChangeCarStatusAsync(int carId, CarStatus newStatus)
        {
            var car = await _carRepo.GetByIdAsync(carId);
            if (car == null) return (false, "Không tìm thấy xe!");

            car.Status = newStatus; // Đổi sang trạng thái Sếp chọn
            car.UpdatedAt = DateTime.Now;

            // Xóa luôn lý do từ chối (nếu có) cho nó sạch sẽ
            car.RejectionReason = null;

            await _carRepo.UpdateAsync(car); // Lưu vào DB
            return (true, $"Đã ép trạng thái xe thành: {newStatus}");
        }

        // DUYỆT HÀNG LOẠT
        public async Task<bool> BulkApproveAsync(List<int> carIds)
        {
            foreach (var id in carIds)
            {
                var car = await _carRepo.GetByIdAsync(id);
                if (car != null && car.Status == CarStatus.PendingApproval)
                {
                    car.Status = CarStatus.Available;
                    car.UpdatedAt = DateTime.Now;
                    await _carRepo.UpdateAsync(car);
                }
            }
            return true; // Hoặc ní có thể viết hàm BulkUpdate trong Repo cho xịn hơn
        }
    }
}