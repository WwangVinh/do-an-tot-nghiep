using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Linq;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;
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
        private readonly ICarPricingVersionRepository _pricingVersionRepo;
        private readonly IUnitOfWork _uow;
        private readonly INotificationService _notiService;

        // ⚠️ CarColorRepository — ní cần tạo interface ICarColorRepository nếu chưa có,
        //    với các method: AddRangeAsync, DeleteByCarIdAsync, GetByCarIdAsync.
        //    Tạm thời tui dùng _context trực tiếp qua _carRepo để minh họa,
        //    nhưng best practice là tách thành repo riêng.
        // private readonly ICarColorRepository _colorRepo;

        // Cached JSON options
        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        // Roles được phép tự quyết Status (không phải gửi duyệt)
        private static readonly string[] PrivilegedRoles = new[]
        {
            "Admin", "ShowroomManager", "SalesManager"
        };

        public CarAdminService(
            ICarRepository carRepo,
            ICarImageRepository imageRepo,
            ICarFeatureRepository carFeatureRepo,
            ICarSpecificationRepository carSpecificationRepo,
            ICarInventoryRepository inventoryRepo,
            ICarPricingVersionRepository pricingVersionRepo,
            IUnitOfWork uow,
            INotificationService notiService
        )
        {
            _carRepo = carRepo;
            _imageRepo = imageRepo;
            _carFeatureRepo = carFeatureRepo;
            _carSpecificationRepo = carSpecificationRepo;
            _inventoryRepo = inventoryRepo;
            _pricingVersionRepo = pricingVersionRepo;
            _uow = uow;
            _notiService = notiService;
        }

        // ==========================================================
        // HELPERS
        // ==========================================================

        private static bool IsPrivileged(string? userRole)
            => userRole != null && PrivilegedRoles.Contains(userRole);

        /// <summary>
        /// Build message lỗi cho user. KHÔNG lộ chi tiết exception ra ngoài
        /// để tránh leak thông tin nhạy cảm (DB schema, connection string...).
        /// </summary>
        private static string BuildSystemErrorMessage(Exception ex)
        {
            // TODO: log đầy đủ ex.GetBaseException() vào ILogger ở đây
            // Ví dụ: _logger.LogError(ex, "Lỗi xử lý xe");

            var baseMsg = ex.GetBaseException()?.Message ?? ex.Message;

            // Chỉ trả message gốc (đã được throw từ business logic) thay vì lộ stack DB
            // Nếu là DbUpdateException hoặc InvalidOperationException thì có thể trả thẳng
            if (ex is InvalidOperationException || ex is ArgumentException)
                return baseMsg;

            // Còn lại trả generic
            return "Lỗi hệ thống khi xử lý dữ liệu xe. Vui lòng thử lại hoặc liên hệ Admin.";
        }

        /// <summary>
        /// Parse ColorsJson thành list CarColor entity. Trả về null nếu JSON sai.
        /// Format: [{"colorName":"Đỏ","hexCode":"#FF0000","imageUrl":"..."}]
        /// </summary>
        private static List<CarColor>? ParseColorsJson(string? colorsJson, out string? error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(colorsJson)) return new List<CarColor>();

            try
            {
                var dtos = JsonSerializer.Deserialize<List<CarColorCreateDto>>(colorsJson, JsonOpts) ?? new();
                return dtos
                    .Where(c => !string.IsNullOrWhiteSpace(c.ColorName))
                    .Select(c => new CarColor
                    {
                        ColorName = c.ColorName.Trim(),
                        HexCode = string.IsNullOrWhiteSpace(c.HexCode) ? null : c.HexCode.Trim(),
                        ImageUrl = string.IsNullOrWhiteSpace(c.ImageUrl) ? null : c.ImageUrl.Trim(),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    })
                    .ToList();
            }
            catch
            {
                error = "ColorsJson không đúng định dạng JSON. " +
                        "Format đúng: [{\"colorName\":\"Đỏ\",\"hexCode\":\"#FF0000\",\"imageUrl\":\"...\"}]";
                return null;
            }
        }

        /// <summary>
        /// Validate CarColorId trong inventory dto có thuộc về danh sách màu của xe không.
        /// Trả về error message nếu sai, null nếu OK.
        /// </summary>
        private static string? ValidateInventoryColors(
            List<CarInventoryCreateDto> inventories,
            List<CarColor> carColors)
        {
            foreach (var inv in inventories)
            {
                if (!inv.CarColorId.HasValue) continue; // backward compat: cho phép null

                if (!carColors.Any(cc => cc.CarColorId == inv.CarColorId.Value))
                {
                    return $"Inventory cho ShowroomId {inv.ShowroomId}: " +
                           $"CarColorId {inv.CarColorId.Value} không thuộc về xe này!";
                }
            }
            return null;
        }

        // ==========================================================
        // 1. GET ALL
        // ==========================================================
        public async Task<object> GetCarsAsync(
            string? search, string? brand, string? color,
            decimal? minPrice, decimal? maxPrice, CarStatus? status,
            string? transmission, string? bodyStyle,
            string? fuelType, string? location,
            bool? isDeleted, int page, int pageSize,
            int? userShowroomId = null,
            string? userRole = null,
            int? createdByUserId = null)
        {
            // ⚠️ FIX: Đẩy filter Sales-thấy-Draft-của-mình xuống Repo bằng cách thêm tham số.
            // Tạm thời giữ filter ở Service để không phải sửa Repo, nhưng cảnh báo:
            // → TotalCount/TotalPages có thể lệch nếu Sales có nhiều xe Draft của người khác.
            // Nếu hệ thống có nhiều Sales, ní nên thêm tham số `excludeOtherSalesDraft` vô Repo.
            var result = await _carRepo.GetAdminCarsAsync(
                search, brand, color, minPrice, maxPrice, status,
                transmission, bodyStyle, fuelType, location,
                isDeleted, page, pageSize, userShowroomId);

            var filteredCars = result.Cars.Where(c =>
                c.Status != CarStatus.Draft
                || userRole == "Admin"
                || userRole == "Manager"
                || userRole == "ShowroomManager"
                || userRole == "SalesManager"
                || (createdByUserId.HasValue && c.CreatedByUserId == createdByUserId.Value)
            ).ToList();

            var adminCars = filteredCars.Select(c =>
            {
                int totalQty = c.CarInventories?.Sum(i => i.Quantity) ?? 0;
                string displayLocation = BuildAdminDisplayLocation(c, totalQty);

                return new
                {
                    c.CarId,
                    c.Name,
                    c.Brand,
                    c.Year,
                    c.Price,
                    c.ImageUrl,
                    Condition = c.Condition.ToString(),
                    Status = c.Status?.ToString() ?? "",
                    c.IsDeleted,
                    TotalQuantity = totalQty,
                    Showrooms = displayLocation,
                    c.BodyStyle,
                    // Trả thêm số màu để FE biết xe có cấu hình màu chưa
                    ColorCount = c.CarColors?.Count ?? 0,
                    CreatedAt = c.CreatedAt?.ToString("dd/MM/yyyy HH:mm"),
                    UpdatedAt = c.UpdatedAt?.ToString("dd/MM/yyyy HH:mm")
                };
            });

            int safePageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, 100);

            return new
            {
                TotalItems = result.TotalCount,
                CurrentPage = page <= 0 ? 1 : page,
                PageSize = safePageSize,
                TotalPages = (int)Math.Ceiling(result.TotalCount / (double)safePageSize),
                Data = adminCars
            };
        }

        private static string BuildAdminDisplayLocation(Car c, int totalQty)
        {
            if (c.Status == CarStatus.COMING_SOON) return "Sắp về";
            if (totalQty == 0) return "Hết hàng";

            if (c.CarInventories == null) return "Đang cập nhật vị trí";

            var activeLocations = c.CarInventories
                .Where(inv => inv.Quantity > 0
                              && inv.Showroom != null
                              && !string.IsNullOrWhiteSpace(inv.Showroom.Province))
                .Select(inv => inv.Showroom.Province)
                .Distinct()
                .ToList();

            if (!activeLocations.Any()) return "Đang cập nhật vị trí";

            string result = string.Join(", ", activeLocations.Take(2));
            if (activeLocations.Count > 2) result += ", ...";
            return result;
        }

        // ==========================================================
        // 2. GET DETAIL
        // ==========================================================
        public async Task<object?> GetCarDetailAsync(int id, string userRole, int? userShowroomId)
        {
            var car = await _carRepo.GetCarDetailForAdminAsync(id);
            if (car == null) return null;

            var allowedInventories = car.CarInventories;
            if (userRole != "Admin" && userShowroomId.HasValue)
            {
                allowedInventories = car.CarInventories?
                    .Where(inv => inv.ShowroomId == userShowroomId.Value)
                    .ToList();
            }

            return new
            {
                car.CarId,
                car.Name,
                car.Brand,
                car.Model,
                car.Year,
                car.Price,
                car.Mileage,
                car.FuelType,
                TotalQuantity = allowedInventories?.Sum(i => i.Quantity) ?? 0,
                car.Transmission,
                car.BodyStyle,
                car.Description,
                car.ImageUrl,
                Condition = car.Condition.ToString(),
                Status = car.Status?.ToString() ?? "",
                car.IsDeleted,
                car.CreatedAt,
                car.UpdatedAt,

                Colors = car.CarColors?.Select(c => new
                {
                    c.CarColorId,
                    c.ColorName,
                    c.HexCode,
                    c.ImageUrl,
                    c.IsActive
                }).ToList(),

                ShowroomDetails = allowedInventories?.Select(inv => new
                {
                    inv.ShowroomId,
                    ShowroomName = inv.Showroom?.Name,
                    ShowroomAddress = inv.Showroom?.FullAddress,
                    Quantity = inv.Quantity,
                    StockStatus = inv.Quantity == 0
                        ? "Hết hàng"
                        : (inv.Quantity < 3 ? "Sắp hết" : "Sẵn có"),
                    // Trả thêm thông tin màu cho lô hàng
                    CarColorId = inv.CarColorId,
                    ColorName = inv.CarColor?.ColorName,
                    HexCode = inv.CarColor?.HexCode
                }).ToList(),

                Images360 = car.CarImages
                    .Where(img => img.Is360Degree == true)
                    .OrderBy(img => img.Title)
                    .Select(i => new
                    {
                        i.CarImageId,
                        i.ImageUrl,
                        FrameOrder = i.Title
                    }).ToList(),

                Specifications = car.CarSpecifications
                    .GroupBy(s => s.Category)
                    .Select(group => new
                    {
                        Category = group.Key,
                        Items = group.Select(i => new { i.SpecName, i.SpecValue }).ToList()
                    }).ToList(),

                Features = car.CarFeatures
                    .Select(cf => new
                    {
                        cf.FeatureId,
                        FeatureName = cf.Feature?.FeatureName,
                        Icon = cf.Feature?.Icon
                    }).ToList(),

                GalleryImages = car.CarImages.Where(img => img.Is360Degree == false)
                    .GroupBy(img => img.ImageType)
                    .Select(group => new
                    {
                        Category = group.Key,
                        Images = group.Select(i => new { i.CarImageId, i.Title, i.Description, i.ImageUrl }).ToList()
                    }).ToList(),

                PricingVersions = car.CarPricingVersions
                    .OrderBy(p => p.SortOrder)
                    .ThenBy(p => p.PricingVersionId)
                    .Select(p => new
                    {
                        p.PricingVersionId,
                        p.VersionName,
                        p.PriceVnd,
                        p.SortOrder,
                        p.IsActive
                    }).ToList()
            };
        }

        // ==========================================================
        // 3. CREATE (đơn giản — không có gallery, pricing version)
        // ==========================================================
        public async Task<(bool Success, string Message, Car? Data)> CreateCarAsync(
            CarCreateDto dto, string userRole, int? userShowroomId, int? createdByUserId = null)
        {
            if (!string.IsNullOrWhiteSpace(dto.Brand)) dto.Brand = dto.Brand.Trim().ToUpper();
            if (!string.IsNullOrWhiteSpace(dto.Name)) dto.Name = dto.Name.Trim();

            int targetShowroomId = (userRole != "Admin" && userShowroomId.HasValue)
                ? userShowroomId.Value : dto.ShowroomId;

            // FIX: thêm SalesManager vô danh sách được tự quyết status (để nhất quán)
            CarStatus finalStatus = IsPrivileged(userRole)
                ? (dto.Status ?? CarStatus.Available)
                : CarStatus.PendingApproval;

            // ===== PARSE COLORS (NEW) =====
            var newColors = ParseColorsJson(dto.ColorsJson, out var colorErr);
            if (newColors == null) return (false, colorErr!, null);

            // 1. LUỒNG XE MỚI: kiểm tra trùng mẫu
            if (dto.Condition == CarCondition.New)
            {
                var existingCar = await _carRepo.GetExistingNewCarAsync(dto.Name, dto.Brand, dto.Year);

                if (existingCar != null)
                {
                    var inventory = await _inventoryRepo.GetInventoryAsync(existingCar.CarId, targetShowroomId);
                    if (inventory != null)
                    {
                        inventory.Quantity += dto.Quantity;
                        inventory.UpdatedAt = DateTime.UtcNow;
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
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                    return (true, $"Mẫu '{existingCar.Name}' đã có trên hệ thống. Tui đã tự động cộng {dto.Quantity} xe vào kho chi nhánh ní!", existingCar);
                }
            }

            // 2. Check trùng listing
            if (await _carRepo.CheckCarListingExistAsync(dto.Name, dto.Brand, dto.Year, (int)dto.Condition, (decimal)(dto.Mileage ?? 0)))
                return (false, "Tin đăng này y hệt một cái khác đã có, ní kiểm tra lại coi!", null);

            var car = new Car
            {
                Name = dto.Name,
                Brand = dto.Brand,
                Year = dto.Year,
                Model = dto.Model,
                Condition = dto.Condition,
                Price = dto.Price,
                FuelType = dto.FuelType,
                Mileage = (decimal)(dto.Mileage ?? 0),
                Description = dto.Description,
                Transmission = dto.Transmission,
                BodyStyle = dto.BodyStyle,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = finalStatus,
                IsDeleted = false,
                CreatedByUserId = createdByUserId,
                // ✨ Gán colors vô navigation collection — EF sẽ tự insert luôn khi AddCarAsync
                CarColors = newColors
            };

            if (dto.ImageFile != null)
            {
                string subFolder = $"Cars/{car.Brand}";
                string targetName = $"{car.Brand}_{car.Name}";
                car.ImageUrl = await FileHelper.UploadFileAsync(dto.ImageFile, subFolder, targetName);
            }
            else car.ImageUrl = "/uploads/Cars/default-car.png";

            await _carRepo.AddCarAsync(car);

            // 3. NHẬP KHO
            if (targetShowroomId > 0)
            {
                await _inventoryRepo.AddInventoryAsync(new CarInventory
                {
                    CarId = car.CarId,
                    ShowroomId = targetShowroomId,
                    Quantity = dto.Quantity,
                    DisplayStatus = (finalStatus == CarStatus.Available) ? "OnDisplay" : "Pending",
                    // Note: CarCreateDto đơn giản không có CarColorId per-inventory.
                    // Nếu muốn phân màu cho inventory thì dùng CreateCarFullAsync với InventoriesJson.
                    UpdatedAt = DateTime.UtcNow
                });
            }

            // 4. FEATURES & SPECS
            if (!string.IsNullOrWhiteSpace(dto.FeatureIds))
            {
                var fIds = dto.FeatureIds.Split(',')
                    .Select(s => s.Trim())
                    .Where(s => int.TryParse(s, out _))
                    .Select(int.Parse)
                    .Distinct()
                    .ToList();
                if (fIds.Any())
                {
                    var carFeatures = fIds.Select(fId => new CarFeature { CarId = car.CarId, FeatureId = fId });
                    await _carFeatureRepo.AddRangeAsync(carFeatures);
                }
            }

            if (!string.IsNullOrWhiteSpace(dto.Specifications))
            {
                var specLines = dto.Specifications.Split(';', StringSplitOptions.RemoveEmptyEntries);
                var carSpecs = specLines.Select(line => line.Split('|'))
                    .Where(p => p.Length == 3)
                    .Select(p => new CarSpecification
                    {
                        CarId = car.CarId,
                        Category = p[0].Trim(),
                        SpecName = p[1].Trim(),
                        SpecValue = p[2].Trim()
                    }).ToList();
                if (carSpecs.Any()) await _carSpecificationRepo.AddRangeAsync(carSpecs);
            }

            if (finalStatus == CarStatus.PendingApproval)
            {
                await _notiService.CreateNotificationAsync(
                    userId: null,
                    showroomId: targetShowroomId,
                    roleTarget: "ShowroomManager",
                    title: "Có xe mới cần duyệt",
                    content: $"Nhân viên vừa đăng mẫu {car.Brand} {car.Name}. Sếp vào duyệt nhé!",
                    actionUrl: $"/admin/cars/approve/{car.CarId}",
                    type: "CarApproval"
                );
            }

            string finalMsg = (finalStatus == CarStatus.Available)
                ? "Đã lên sàn con xe mới tinh!"
                : "Đã tạo yêu cầu, đợi sếp gật đầu là xe lên sóng nha!";
            return (true, finalMsg, car);
        }

        // ==========================================================
        // 3B. CREATE FULL
        // ==========================================================
        public async Task<(bool Success, string Message, Car? Data)> CreateCarFullAsync(
            CarCreateFullDto dto, string userRole, int? userShowroomId, int? createdByUserId = null)
        {
            if (!string.IsNullOrWhiteSpace(dto.Brand)) dto.Brand = dto.Brand.Trim().ToUpper();
            if (!string.IsNullOrWhiteSpace(dto.Name)) dto.Name = dto.Name.Trim();

            int targetShowroomId = (userRole != "Admin" && userShowroomId.HasValue)
                ? userShowroomId.Value : dto.ShowroomId;

            CarStatus finalStatus = IsPrivileged(userRole)
                ? (dto.Status ?? CarStatus.Available)
                : CarStatus.PendingApproval;

            // ===== PARSE TẤT CẢ JSON (validate sớm trước khi vô transaction) =====
            var newColors = ParseColorsJson(dto.ColorsJson, out var colorErr);
            if (newColors == null) return (false, colorErr!, null);

            List<CarSpecificationCreateDto> specs = new();
            if (!string.IsNullOrWhiteSpace(dto.SpecificationsJson))
            {
                try { specs = JsonSerializer.Deserialize<List<CarSpecificationCreateDto>>(dto.SpecificationsJson, JsonOpts) ?? new(); }
                catch { return (false, "SpecificationsJson không đúng định dạng JSON.", null); }
            }
            else if (!string.IsNullOrWhiteSpace(dto.Specifications))
            {
                var specLines = dto.Specifications.Split(';', StringSplitOptions.RemoveEmptyEntries);
                specs = specLines.Select(line => line.Split('|'))
                    .Where(p => p.Length == 3)
                    .Select(p => new CarSpecificationCreateDto
                    {
                        Category = p[0].Trim(),
                        SpecName = p[1].Trim(),
                        SpecValue = p[2].Trim()
                    }).ToList();
            }

            List<CarPricingVersionCreateDto> pricingVersions = new();
            if (!string.IsNullOrWhiteSpace(dto.PricingVersionsJson))
            {
                try { pricingVersions = JsonSerializer.Deserialize<List<CarPricingVersionCreateDto>>(dto.PricingVersionsJson, JsonOpts) ?? new(); }
                catch { return (false, "PricingVersionsJson không đúng định dạng JSON.", null); }
            }

            List<CarInventoryCreateDto> inventories = new();
            if (!string.IsNullOrWhiteSpace(dto.InventoriesJson))
            {
                try { inventories = JsonSerializer.Deserialize<List<CarInventoryCreateDto>>(dto.InventoriesJson, JsonOpts) ?? new(); }
                catch { return (false, "InventoriesJson không đúng định dạng JSON.", null); }
            }

            List<CarImageMetaDto> galleryMetas = new();
            if (!string.IsNullOrWhiteSpace(dto.GalleryMetasJson))
            {
                try { galleryMetas = JsonSerializer.Deserialize<List<CarImageMetaDto>>(dto.GalleryMetasJson, JsonOpts) ?? new(); }
                catch { return (false, "GalleryMetasJson không đúng định dạng JSON.", null); }
            }

            if (dto.GalleryFiles != null && dto.GalleryFiles.Any())
            {
                if (galleryMetas.Count != dto.GalleryFiles.Count)
                    return (false, "GalleryMetasJson phải có số phần tử đúng bằng số file trong GalleryFiles.", null);
            }

            if (await _carRepo.CheckCarListingExistAsync(dto.Name, dto.Brand, dto.Year, (int)dto.Condition, (decimal)(dto.Mileage ?? 0)))
                return (false, "Tin đăng này y hệt một cái khác đã có, ní kiểm tra lại coi!", null);

            Car? createdCar = null;
            try
            {
                await _uow.RunInTransactionAsync(async () =>
                {
                    // 1) Cars (gán luôn CarColors qua navigation)
                    var car = new Car
                    {
                        Name = dto.Name,
                        Brand = dto.Brand,
                        Year = dto.Year,
                        Model = dto.Model,
                        Condition = dto.Condition,
                        Price = dto.Price,
                        FuelType = dto.FuelType,
                        Mileage = (decimal)(dto.Mileage ?? 0),
                        Description = dto.Description,
                        Transmission = dto.Transmission,
                        BodyStyle = dto.BodyStyle,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Status = finalStatus,
                        IsDeleted = false,
                        CreatedByUserId = createdByUserId,
                        CarColors = newColors // ✨ Insert kèm xe luôn
                    };

                    if (dto.ImageFile != null)
                    {
                        string subFolder = $"Cars/{car.Brand}";
                        string targetName = $"{car.Brand}_{car.Name}";
                        car.ImageUrl = await FileHelper.UploadFileAsync(dto.ImageFile, subFolder, targetName);
                    }
                    else
                    {
                        car.ImageUrl = "/uploads/Cars/default-car.png";
                    }

                    await _carRepo.AddCarAsync(car);
                    createdCar = car;

                    // ✨ VALIDATE: nếu inventories có CarColorId thì phải khớp với CarColors vừa tạo
                    var colorErrIn = ValidateInventoryColors(inventories, car.CarColors.ToList());
                    if (colorErrIn != null) throw new InvalidOperationException(colorErrIn);

                    // 2) CarInventories — TRUYỀN CarColorId
                    if (inventories.Any())
                    {
                        if (userRole != "Admin" && userShowroomId.HasValue
                            && inventories.Any(i => i.ShowroomId != userShowroomId.Value))
                            throw new InvalidOperationException("Không được tạo tồn kho cho showroom khác chi nhánh của mình.");

                        foreach (var inv in inventories.Where(i => i.ShowroomId > 0))
                        {
                            await _inventoryRepo.AddInventoryAsync(new CarInventory
                            {
                                CarId = car.CarId,
                                ShowroomId = inv.ShowroomId,
                                Quantity = inv.Quantity,
                                DisplayStatus = string.IsNullOrWhiteSpace(inv.DisplayStatus)
                                    ? ((finalStatus == CarStatus.Available) ? "OnDisplay" : "Pending")
                                    : inv.DisplayStatus.Trim(),
                                CarColorId = inv.CarColorId, // ✨ FIX: gán màu cho lô hàng
                                UpdatedAt = DateTime.UtcNow
                            });
                        }
                    }
                    else if (targetShowroomId > 0)
                    {
                        await _inventoryRepo.AddInventoryAsync(new CarInventory
                        {
                            CarId = car.CarId,
                            ShowroomId = targetShowroomId,
                            Quantity = dto.Quantity,
                            DisplayStatus = (finalStatus == CarStatus.Available) ? "OnDisplay" : "Pending",
                            UpdatedAt = DateTime.UtcNow
                        });
                    }

                    // 3) CarFeatures
                    if (!string.IsNullOrWhiteSpace(dto.FeatureIds))
                    {
                        var fIds = dto.FeatureIds.Split(',')
                            .Select(s => s.Trim())
                            .Where(s => int.TryParse(s, out _))
                            .Select(int.Parse)
                            .Distinct()
                            .ToList();

                        if (fIds.Any())
                        {
                            var carFeatures = fIds.Select(fId => new CarFeature { CarId = car.CarId, FeatureId = fId });
                            await _carFeatureRepo.AddRangeAsync(carFeatures);
                        }
                    }

                    // 4) CarSpecifications
                    if (specs.Any())
                    {
                        var entities = specs
                            .Where(s => !string.IsNullOrWhiteSpace(s.Category)
                                        && !string.IsNullOrWhiteSpace(s.SpecName)
                                        && !string.IsNullOrWhiteSpace(s.SpecValue))
                            .Select(s => new CarSpecification
                            {
                                CarId = car.CarId,
                                Category = s.Category.Trim(),
                                SpecName = s.SpecName.Trim(),
                                SpecValue = s.SpecValue.Trim()
                            })
                            .ToList();
                        if (entities.Any()) await _carSpecificationRepo.AddRangeAsync(entities);
                    }

                    // 5) PricingVersions
                    if (pricingVersions.Any())
                    {
                        var normalized = pricingVersions
                            .Where(p => !string.IsNullOrWhiteSpace(p.VersionName))
                            .Select((p, idx) => new CarPricingVersionCreateDto
                            {
                                VersionName = p.VersionName.Trim(),
                                PriceVnd = p.PriceVnd,
                                SortOrder = p.SortOrder == 0 ? (idx + 1) : p.SortOrder,
                                IsActive = p.IsActive
                            }).ToList();

                        foreach (var pv in normalized)
                        {
                            await _pricingVersionRepo.AddAsync(new CarPricingVersion
                            {
                                CarId = car.CarId,
                                VersionName = pv.VersionName,
                                PriceVnd = pv.PriceVnd,
                                SortOrder = pv.SortOrder,
                                IsActive = pv.IsActive,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            });
                        }

                        var activePrices = normalized.Where(x => x.IsActive).Select(x => x.PriceVnd).ToList();
                        var minPrice = activePrices.Any() ? activePrices.Min() : normalized.Min(x => x.PriceVnd);
                        car.Price = minPrice;
                        car.UpdatedAt = DateTime.UtcNow;
                        await _carRepo.UpdateAsync(car);
                    }

                    // 6) Gallery
                    if (dto.GalleryFiles != null && dto.GalleryFiles.Any())
                    {
                        string subFolder = $"Cars/{car.Brand}";
                        string targetName = $"{car.Brand}_{car.Name}";

                        for (int i = 0; i < dto.GalleryFiles.Count; i++)
                        {
                            var file = dto.GalleryFiles[i];
                            if (file == null || file.Length == 0) continue;

                            var meta = (i < galleryMetas.Count) ? galleryMetas[i] : null;
                            if (meta == null)
                                throw new InvalidOperationException($"GalleryMetasJson thiếu phần tử ở vị trí #{i + 1}.");

                            string imagePath = await FileHelper.UploadFileAsync(file, subFolder, targetName);
                            string fileHash = FileHelper.GetFileHash(file);

                            await _imageRepo.AddCarImageAsync(new CarImage
                            {
                                CarId = car.CarId,
                                ImageUrl = imagePath,
                                Is360Degree = false,
                                IsMainImage = meta.IsMainImage ?? false,
                                ImageType = string.IsNullOrWhiteSpace(meta.ImageType) ? null : meta.ImageType.Trim(),
                                Title = string.IsNullOrWhiteSpace(meta.Title) ? null : meta.Title.Trim(),
                                Description = string.IsNullOrWhiteSpace(meta.Description) ? null : meta.Description.Trim(),
                                FileHash = string.IsNullOrWhiteSpace(fileHash) ? null : fileHash,
                                CreatedAt = DateTime.UtcNow
                            });
                        }
                    }

                    // 7) Notification
                    if (finalStatus == CarStatus.PendingApproval)
                    {
                        await _notiService.CreateNotificationAsync(
                            userId: null,
                            showroomId: targetShowroomId,
                            roleTarget: "ShowroomManager",
                            title: "Có xe mới cần duyệt",
                            content: $"Nhân viên vừa đăng mẫu {car.Brand} {car.Name}. Sếp vào duyệt nhé!",
                            actionUrl: $"/admin/cars/approve/{car.CarId}",
                            type: "CarApproval"
                        );
                    }
                });

                string msg = (finalStatus == CarStatus.Available)
                    ? "Đã tạo xe (full) và lên sàn thành công!"
                    : "Đã tạo xe (full). Đang chờ duyệt để lên sàn!";

                return (true, msg, createdCar);
            }
            catch (Exception ex)
            {
                return (false, BuildSystemErrorMessage(ex), null);
            }
        }

        // ==========================================================
        // 3C. UPDATE FULL
        // ==========================================================
        public async Task<(bool Success, string Message, Car? Data)> UpdateCarFullAsync(
            int id, CarCreateFullDto dto, string userRole, int? userShowroomId)
        {
            if (!string.IsNullOrWhiteSpace(dto.Brand)) dto.Brand = dto.Brand.Trim().ToUpper();
            if (!string.IsNullOrWhiteSpace(dto.Name)) dto.Name = dto.Name.Trim();

            // ⚠️ Dùng GetCarDetailForAdminAsync để có Include đầy đủ (CarColors, etc.)
            var oldCar = await _carRepo.GetCarDetailForAdminAsync(id);
            if (oldCar == null) return (false, "Không tìm thấy xe này trong hệ thống!", null);

            int targetShowroomId = (userRole != "Admin" && userShowroomId.HasValue)
                ? userShowroomId.Value : dto.ShowroomId;

            CarStatus finalStatus = IsPrivileged(userRole)
                ? (dto.Status ?? oldCar.Status ?? CarStatus.Draft)
                : CarStatus.PendingApproval;

            // ===== PARSE JSON =====
            var newColors = ParseColorsJson(dto.ColorsJson, out var colorErr);
            if (newColors == null) return (false, colorErr!, null);

            List<CarSpecificationCreateDto> specs = new();
            if (!string.IsNullOrWhiteSpace(dto.SpecificationsJson))
            {
                try { specs = JsonSerializer.Deserialize<List<CarSpecificationCreateDto>>(dto.SpecificationsJson, JsonOpts) ?? new(); }
                catch { return (false, "SpecificationsJson không đúng định dạng JSON.", null); }
            }
            else if (!string.IsNullOrWhiteSpace(dto.Specifications))
            {
                var specLines = dto.Specifications.Split(';', StringSplitOptions.RemoveEmptyEntries);
                specs = specLines.Select(line => line.Split('|'))
                    .Where(p => p.Length == 3)
                    .Select(p => new CarSpecificationCreateDto
                    {
                        Category = p[0].Trim(),
                        SpecName = p[1].Trim(),
                        SpecValue = p[2].Trim()
                    }).ToList();
            }

            List<CarPricingVersionCreateDto> pricingVersions = new();
            if (!string.IsNullOrWhiteSpace(dto.PricingVersionsJson))
            {
                try { pricingVersions = JsonSerializer.Deserialize<List<CarPricingVersionCreateDto>>(dto.PricingVersionsJson, JsonOpts) ?? new(); }
                catch { return (false, "PricingVersionsJson không đúng định dạng JSON.", null); }
            }

            List<CarInventoryCreateDto> inventories = new();
            if (!string.IsNullOrWhiteSpace(dto.InventoriesJson))
            {
                try { inventories = JsonSerializer.Deserialize<List<CarInventoryCreateDto>>(dto.InventoriesJson, JsonOpts) ?? new(); }
                catch { return (false, "InventoriesJson không đúng định dạng JSON.", null); }
            }

            List<CarImageMetaDto> galleryMetas = new();
            if (!string.IsNullOrWhiteSpace(dto.GalleryMetasJson))
            {
                try { galleryMetas = JsonSerializer.Deserialize<List<CarImageMetaDto>>(dto.GalleryMetasJson, JsonOpts) ?? new(); }
                catch { return (false, "GalleryMetasJson không đúng định dạng JSON.", null); }
            }

            if (dto.GalleryFiles != null && dto.GalleryFiles.Any())
            {
                if (galleryMetas.Count != dto.GalleryFiles.Count)
                    return (false, "GalleryMetasJson phải có số phần tử đúng bằng số file trong GalleryFiles.", null);
            }

            Car? updated = null;
            try
            {
                await _uow.RunInTransactionAsync(async () =>
                {
                    // 1) Cars
                    oldCar.Name = dto.Name;
                    oldCar.Brand = dto.Brand;
                    oldCar.Year = dto.Year;
                    oldCar.Model = dto.Model;
                    oldCar.FuelType = dto.FuelType;
                    oldCar.Mileage = (decimal)(dto.Mileage ?? 0);
                    oldCar.Description = dto.Description;
                    oldCar.Transmission = dto.Transmission;
                    oldCar.BodyStyle = dto.BodyStyle;
                    oldCar.Condition = dto.Condition;
                    oldCar.Status = finalStatus;
                    oldCar.UpdatedAt = DateTime.UtcNow;

                    if (dto.ImageFile != null)
                    {
                        if (!string.IsNullOrEmpty(oldCar.ImageUrl)
                            && !oldCar.ImageUrl.StartsWith("http")
                            && !oldCar.ImageUrl.Contains("default-car"))
                        {
                            FileHelper.DeleteFile(oldCar.ImageUrl);
                        }

                        string subFolder = $"Cars/{oldCar.Brand}";
                        string targetName = $"{oldCar.Brand}_{oldCar.Name}";
                        oldCar.ImageUrl = await FileHelper.UploadFileAsync(dto.ImageFile, subFolder, targetName);
                    }

                    // 1B) ✨ COLORS — replace all (chỉ khi FE có gửi ColorsJson)
                    // Nếu FE không gửi ColorsJson (null/empty) → giữ nguyên màu cũ.
                    // Nếu FE gửi mảng rỗng [] → xóa hết màu.
                    if (!string.IsNullOrWhiteSpace(dto.ColorsJson))
                    {
                        // Trước khi xóa màu cũ: phải xóa hoặc null hóa CarColorId trên CarInventory
                        // để tránh FK error. Đơn giản nhất: null hóa CarColorId trên inventories cũ.
                        var oldInvWithColors = await _inventoryRepo.GetInventoriesByCarIdAsync(id);
                        foreach (var inv in oldInvWithColors.Where(i => i.CarColorId.HasValue))
                        {
                            inv.CarColorId = null;
                            inv.UpdatedAt = DateTime.UtcNow;
                            await _inventoryRepo.UpdateInventoryAsync(inv);
                        }

                        // Xóa màu cũ rồi thêm màu mới
                        if (oldCar.CarColors != null)
                        {
                            foreach (var oldColor in oldCar.CarColors.ToList())
                            {
                                oldCar.CarColors.Remove(oldColor);
                            }
                        }
                        foreach (var newColor in newColors)
                        {
                            newColor.CarId = id;
                            oldCar.CarColors.Add(newColor);
                        }
                    }

                    // 2) Inventories (replace all)
                    await _inventoryRepo.DeleteInventoriesByCarIdAsync(id);
                    if (inventories.Any())
                    {
                        if (userRole != "Admin" && userShowroomId.HasValue
                            && inventories.Any(i => i.ShowroomId != userShowroomId.Value))
                            throw new InvalidOperationException("Không được tạo tồn kho cho showroom khác chi nhánh của mình.");

                        // Validate màu trong inventory phải thuộc về xe (sau khi đã update CarColors)
                        var currentColors = oldCar.CarColors?.ToList() ?? new List<CarColor>();
                        var colorErrIn = ValidateInventoryColors(inventories, currentColors);
                        if (colorErrIn != null) throw new InvalidOperationException(colorErrIn);

                        foreach (var inv in inventories.Where(i => i.ShowroomId > 0))
                        {
                            await _inventoryRepo.AddInventoryAsync(new CarInventory
                            {
                                CarId = id,
                                ShowroomId = inv.ShowroomId,
                                Quantity = inv.Quantity,
                                DisplayStatus = string.IsNullOrWhiteSpace(inv.DisplayStatus)
                                    ? ((finalStatus == CarStatus.Available) ? "OnDisplay" : "Pending")
                                    : inv.DisplayStatus.Trim(),
                                CarColorId = inv.CarColorId, // ✨ FIX
                                UpdatedAt = DateTime.UtcNow
                            });
                        }
                    }
                    else if (targetShowroomId > 0)
                    {
                        await _inventoryRepo.AddInventoryAsync(new CarInventory
                        {
                            CarId = id,
                            ShowroomId = targetShowroomId,
                            Quantity = dto.Quantity,
                            DisplayStatus = (finalStatus == CarStatus.Available) ? "OnDisplay" : "Pending",
                            UpdatedAt = DateTime.UtcNow
                        });
                    }

                    // 3) Features
                    await _carFeatureRepo.DeleteByCarIdAsync(id);
                    if (!string.IsNullOrWhiteSpace(dto.FeatureIds))
                    {
                        var fIds = dto.FeatureIds.Split(',')
                            .Select(s => s.Trim())
                            .Where(s => int.TryParse(s, out _))
                            .Select(int.Parse)
                            .Distinct()
                            .ToList();

                        if (fIds.Any())
                        {
                            var entities = fIds.Select(fid => new CarFeature { CarId = id, FeatureId = fid }).ToList();
                            await _carFeatureRepo.AddRangeAsync(entities);
                        }
                    }

                    // 4) Specs
                    await _carSpecificationRepo.DeleteByCarIdAsync(id);
                    if (specs.Any())
                    {
                        var entities = specs
                            .Where(s => !string.IsNullOrWhiteSpace(s.Category)
                                        && !string.IsNullOrWhiteSpace(s.SpecName)
                                        && !string.IsNullOrWhiteSpace(s.SpecValue))
                            .Select(s => new CarSpecification
                            {
                                CarId = id,
                                Category = s.Category.Trim(),
                                SpecName = s.SpecName.Trim(),
                                SpecValue = s.SpecValue.Trim()
                            }).ToList();
                        if (entities.Any()) await _carSpecificationRepo.AddRangeAsync(entities);
                    }

                    // 5) Pricing
                    await _pricingVersionRepo.DeleteByCarIdAsync(id);
                    if (pricingVersions.Any())
                    {
                        var entities = pricingVersions
                            .Where(p => !string.IsNullOrWhiteSpace(p.VersionName))
                            .Select((p, idx) => new CarPricingVersion
                            {
                                CarId = id,
                                VersionName = p.VersionName.Trim(),
                                PriceVnd = p.PriceVnd,
                                SortOrder = p.SortOrder <= 0 ? (idx + 1) : p.SortOrder,
                                IsActive = p.IsActive
                            }).ToList();
                        if (entities.Any()) await _pricingVersionRepo.AddRangeAsync(entities);
                    }

                    var active = await _pricingVersionRepo.GetAllAsync(id, true);
                    if (active.Any())
                    {
                        oldCar.Price = active.Min(x => x.PriceVnd);
                    }
                    else
                    {
                        oldCar.Price = dto.Price;
                    }

                    // 6) Gallery
                    if (dto.GalleryFiles != null && dto.GalleryFiles.Any())
                    {
                        await _imageRepo.DeleteAllGalleryImagesByCarIdAsync(id);
                        for (int i = 0; i < dto.GalleryFiles.Count; i++)
                        {
                            var file = dto.GalleryFiles[i];
                            var meta = galleryMetas[i];
                            string folder = $"Cars/{oldCar.Brand}/{oldCar.Brand}_{oldCar.Name}/Gallery";
                            var url = await FileHelper.UploadFileAsync(file, folder, meta.Title ?? $"img_{i + 1}");
                            await _imageRepo.AddCarImageAsync(new CarImage
                            {
                                CarId = id,
                                Title = meta.Title,
                                Description = meta.Description,
                                ImageType = meta.ImageType,
                                ImageUrl = url,
                                Is360Degree = false
                            });
                        }
                    }

                    await _carRepo.UpdateCarAsync(oldCar);
                    updated = oldCar;
                });

                return (true, "Cập nhật xe (full) thành công!", updated);
            }
            catch (Exception ex)
            {
                return (false, BuildSystemErrorMessage(ex), null);
            }
        }

        // ==========================================================
        // 4. UPDATE (đơn giản)
        // ==========================================================
        public async Task<(bool Success, string Message, Car? Car)> UpdateCarAsync(
            int id, CarUpdateDto dto, string userRole, int? userShowroomId)
        {
            // ⚠️ Dùng GetCarDetailForAdminAsync để có CarColors. GetByIdAsync chỉ Include CarColors,
            // nhưng GetCarDetailForAdminAsync Include hết → an toàn hơn cho Update.
            var car = await _carRepo.GetCarDetailForAdminAsync(id);
            if (car == null) return (false, "Không tìm thấy xe!", null);

            if (userRole != "Admin" && userShowroomId.HasValue)
            {
                var inventories = await _inventoryRepo.GetInventoriesByCarIdAsync(id);
                if (!inventories.Any(i => i.ShowroomId == userShowroomId.Value))
                {
                    return (false, "Sếp ơi, xe này không thuộc quyền quản lý của chi nhánh mình!", null);
                }
            }

            string cleanBrand = dto.Brand?.Trim().ToUpper() ?? "";
            string cleanName = dto.Name?.Trim() ?? "";

            if (dto.Condition == CarCondition.New)
            {
                var duplicateModel = await _carRepo.GetExistingNewCarAsync(cleanName, cleanBrand, dto.Year);
                if (duplicateModel != null && duplicateModel.CarId != id)
                {
                    return (false, $"Ní ơi, mẫu '{cleanName}' đời {dto.Year} đã có trên hệ thống rồi!", null);
                }
            }

            // ===== PARSE COLORS (NEW) =====
            var newColors = ParseColorsJson(dto.ColorsJson, out var colorErr);
            if (newColors == null) return (false, colorErr!, null);

            // Cập nhật field cơ bản
            car.Name = cleanName;
            car.Brand = cleanBrand;
            car.Model = dto.Model;
            car.Price = (decimal)dto.Price;
            car.Year = dto.Year;
            car.Description = dto.Description;
            car.Condition = dto.Condition; // ⚠️ Theo bản fix DTO, Condition đã là CarCondition enum
            car.FuelType = dto.FuelType;
            car.Mileage = (decimal)dto.Mileage;
            car.Transmission = dto.Transmission;
            car.BodyStyle = dto.BodyStyle;
            car.UpdatedAt = DateTime.UtcNow;

            // Logic Status
            if (IsPrivileged(userRole))
            {
                if (dto.Status.HasValue) car.Status = dto.Status.Value;
            }
            else if (userRole == "ShowroomSales" || userRole == "Sales"
                     || userRole == "Technician" || userRole == "Staff")
            {
                if (dto.Status == CarStatus.PendingApproval)
                {
                    car.Status = CarStatus.PendingApproval;
                    car.RejectionReason = null;
                }
                else
                {
                    car.Status = CarStatus.Draft;
                }
            }

            if (await _carRepo.CheckCarListingExistAsync(car.Name, car.Brand, car.Year, (int)car.Condition, (decimal)car.Mileage, id))
            {
                return (false, "Ní sửa gì mà nó trùng khít với một tin đăng khác vậy?", null);
            }

            try
            {
                // Ảnh chính
                if (dto.ImageFile != null)
                {
                    string subFolder = $"Cars/{car.Brand}";
                    string targetName = $"{car.Brand}_{car.Name}";
                    string newImageUrl = await FileHelper.UploadFileAsync(dto.ImageFile, subFolder, targetName);

                    if (!string.IsNullOrEmpty(car.ImageUrl) && !car.ImageUrl.Contains("default-car"))
                    {
                        FileHelper.DeleteFile(car.ImageUrl);
                    }
                    car.ImageUrl = newImageUrl;
                }

                // ✨ COLORS — chỉ replace khi FE gửi ColorsJson
                if (!string.IsNullOrWhiteSpace(dto.ColorsJson))
                {
                    // Null hóa CarColorId trên inventory cũ trước khi xóa màu
                    var oldInvWithColors = await _inventoryRepo.GetInventoriesByCarIdAsync(id);
                    foreach (var inv in oldInvWithColors.Where(i => i.CarColorId.HasValue))
                    {
                        inv.CarColorId = null;
                        inv.UpdatedAt = DateTime.UtcNow;
                        await _inventoryRepo.UpdateInventoryAsync(inv);
                    }

                    if (car.CarColors != null)
                    {
                        foreach (var oldColor in car.CarColors.ToList())
                        {
                            car.CarColors.Remove(oldColor);
                        }
                    }
                    foreach (var nc in newColors)
                    {
                        nc.CarId = id;
                        car.CarColors.Add(nc);
                    }
                }

                // Specs & Features
                await _carSpecificationRepo.DeleteByCarIdAsync(id);
                if (!string.IsNullOrWhiteSpace(dto.Specifications))
                {
                    var specLines = dto.Specifications.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    var newSpecs = specLines.Select(line => line.Split('|'))
                        .Where(p => p.Length == 3)
                        .Select(p => new CarSpecification
                        {
                            CarId = id,
                            Category = p[0].Trim(),
                            SpecName = p[1].Trim(),
                            SpecValue = p[2].Trim()
                        }).ToList();
                    if (newSpecs.Any()) await _carSpecificationRepo.AddRangeAsync(newSpecs);
                }

                await _carFeatureRepo.DeleteByCarIdAsync(id);
                if (!string.IsNullOrWhiteSpace(dto.FeatureIds))
                {
                    var featureIds = dto.FeatureIds.Split(',')
                        .Select(s => s.Trim())
                        .Where(s => int.TryParse(s, out _))
                        .Select(int.Parse)
                        .ToList();
                    var newCarFeatures = featureIds.Select(fId => new CarFeature { CarId = id, FeatureId = fId }).ToList();
                    await _carFeatureRepo.AddRangeAsync(newCarFeatures);
                }

                // Save
                await _carRepo.UpdateAsync(car);

                // Inventory
                if (dto.ShowroomId > 0)
                {
                    var inventory = await _inventoryRepo.GetInventoryAsync(id, dto.ShowroomId);
                    if (inventory != null)
                    {
                        inventory.Quantity = dto.Quantity;
                        inventory.DisplayStatus = dto.Quantity <= 0 ? "Out of stock" : "OnDisplay";
                        inventory.UpdatedAt = DateTime.UtcNow;
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
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                }

                if ((userRole == "ShowroomSales" || userRole == "Staff")
                    && car.Status == CarStatus.PendingApproval)
                {
                    await _notiService.CreateNotificationAsync(
                        userId: null,
                        showroomId: dto.ShowroomId,
                        roleTarget: "ShowroomManager",
                        title: "Có bản cập nhật xe cần duyệt 📝",
                        content: $"Nhân viên vừa sửa và nộp lại thông tin mẫu {car.Brand} {car.Name}.",
                        actionUrl: $"/admin/cars/approve/{car.CarId}",
                        type: "CarApproval"
                    );
                }

                await SyncCarStatusAsync(id);
                return (true, "Cập nhật xe và quản lý mẫu hệ thống thành công!", car);
            }
            catch (Exception ex)
            {
                return (false, BuildSystemErrorMessage(ex), null);
            }
        }

        // ==========================================================
        // 5. UPLOAD GALLERY
        // ==========================================================
        public async Task<(bool Success, string Message, object? Data)> UploadGalleryImagesAsync(
            int carId, List<IFormFile> files, List<string>? titles, List<string>? descriptions, string imageType)
        {
            var car = await _carRepo.GetCarByIdAsync(carId);
            if (car == null) return (false, "Không tìm thấy xe!", null);

            var uploadedImages = new List<CarImage>();
            int skippedCount = 0;
            var existingImages = await _imageRepo.GetCarImagesAsync(carId);

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

                string? currentTitle = (titles != null && i < titles.Count) ? titles[i] : null;
                string? currentDesc = (descriptions != null && i < descriptions.Count) ? descriptions[i] : null;

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
                    Description = currentDesc,
                    FileHash = fileHash,
                    CreatedAt = DateTime.UtcNow
                };

                await _imageRepo.AddCarImageAsync(carImage);
                uploadedImages.Add(carImage);
                existingImages.Add(carImage);
            }

            var responseData = uploadedImages.Select(img => new
            {
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
            return await _imageRepo.UpdateImageDescriptionAsync(imageId, description, title);
        }

        // ==========================================================
        // 6. UPLOAD 360
        // ==========================================================
        public async Task<(bool Success, string Message)> Upload360ImagesAsync(int carId, List<IFormFile> files)
        {
            var car = await _carRepo.GetCarByIdAsync(carId);
            if (car == null) return (false, "Xe không tồn tại.");

            if (files == null || files.Count == 0)
            {
                await _imageRepo.DeleteAll360ImagesByCarIdAsync(carId);
                return (true, "Đã xóa sạch dải phim 360 của xe này.");
            }

            string subFolder = $"Cars/{car.Brand}/{car.Brand}_{car.Name}/360";
            await _imageRepo.DeleteAll360ImagesByCarIdAsync(carId);

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (file.Length > 0)
                {
                    string targetName = $"frame_{i + 1}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
                    string imagePath = await FileHelper.UploadFileAsync(file, subFolder, targetName);

                    var carImage = new CarImage
                    {
                        CarId = carId,
                        ImageUrl = imagePath,
                        Is360Degree = true,
                        Title = (i + 1).ToString(),
                        CreatedAt = DateTime.UtcNow
                    };
                    await _imageRepo.AddCarImageAsync(carImage);
                }
            }

            return (true, $"Tải lên thành công dải phim {files.Count} chưởng!");
        }

        // ==========================================================
        // 7. DELETE IMAGE
        // ==========================================================
        public async Task<bool> DeleteCarImageAsync(int imageId)
        {
            var image = await _imageRepo.GetCarImageByIdAsync(imageId);
            if (image == null) return false;

            if (!string.IsNullOrEmpty(image.ImageUrl)
                && !image.ImageUrl.Contains("default-car")
                && !image.ImageUrl.StartsWith("http"))
            {
                FileHelper.DeleteFile(image.ImageUrl);
            }

            await _imageRepo.DeleteCarImageAsync(imageId);
            return true;
        }

        // ==========================================================
        // 8. SOFT DELETE
        // ==========================================================
        public async Task<bool> SoftDeleteCarAsync(int id, int deletedByUserId)
        {
            var car = await _carRepo.GetCarByIdAsync(id);
            if (car == null || car.IsDeleted == true) return false;

            car.IsDeleted = true;
            car.DeletedAt = DateTime.UtcNow;
            car.DeletedBy = deletedByUserId;
            await _carRepo.UpdateCarAsync(car);
            return true;
        }

        // ==========================================================
        // 9. RESTORE
        // ==========================================================
        public async Task<bool> RestoreCarAsync(int id)
        {
            var car = await _carRepo.GetCarByIdAsync(id);
            if (car == null || car.IsDeleted == false) return false;

            car.IsDeleted = false;
            car.UpdatedAt = DateTime.UtcNow;
            await _carRepo.UpdateCarAsync(car);
            return true;
        }

        // ==========================================================
        // 10. HARD DELETE
        // ==========================================================
        public async Task<bool> HardDeleteCarAsync(int id, string userRole)
        {
            if (userRole != "Admin") return false;

            var car = await _carRepo.GetCarDetailForAdminAsync(id);
            if (car == null) return false;

            if (car.IsDeleted != true && car.Status != CarStatus.Draft) return false;

            // ===== A. Xóa file vật lý =====
            var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

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

            if (!string.IsNullOrEmpty(car.ImageUrl)
                && !car.ImageUrl.Contains("default-car")
                && !car.ImageUrl.StartsWith("http"))
            {
                var mainImagePath = Path.Combine(wwwrootPath, car.ImageUrl.TrimStart('/'));
                if (File.Exists(mainImagePath)) File.Delete(mainImagePath);
            }

            try
            {
                string brandFolder = car.Brand?.Trim().ToUpper() ?? "UNKNOWN";
                string targetName = $"{brandFolder}_{car.Name?.Trim()}";
                string cleanTargetName = string.Join("_", targetName.Split(Path.GetInvalidFileNameChars())).Replace(" ", "_");
                var carFolderPath = Path.Combine(wwwrootPath, "uploads", "Cars", brandFolder, cleanTargetName);

                if (Directory.Exists(carFolderPath))
                {
                    Directory.Delete(carFolderPath, true);
                }
            }
            catch { /* ignore */ }

            // ===== B. Xóa DB (theo thứ tự FK) =====
            await _imageRepo.DeleteAllImagesByCarIdAsync(id);
            await _carFeatureRepo.DeleteByCarIdAsync(id);
            await _carSpecificationRepo.DeleteByCarIdAsync(id);

            // ✨ FIX: phải xóa Inventories TRƯỚC CarColors (vì Inventory ref CarColor)
            await _inventoryRepo.DeleteInventoriesByCarIdAsync(id);

            // ✨ FIX: xóa CarColors (FK với Car) — ní cần thêm method này vô ICarColorRepository
            //   Hoặc tạm thời xóa qua oldCar.CarColors clear + UpdateAsync
            if (car.CarColors != null && car.CarColors.Any())
            {
                foreach (var cc in car.CarColors.ToList())
                {
                    car.CarColors.Remove(cc);
                }
                // Save để EF xóa khỏi DB
                await _carRepo.UpdateAsync(car);
            }

            // ✨ FIX: xóa CarPricingVersions (FK với Car)
            await _pricingVersionRepo.DeleteByCarIdAsync(id);

            // ⚠️ LƯU Ý: Theo bản sửa CarRepository, DeleteCarAsync giờ là SOFT DELETE.
            // Để hard delete thật sự, ní cần thêm 1 method mới trong ICarRepository:
            //   Task<bool> HardDeleteCarAsync(int id);
            // Tạm thời dùng DeleteCarAsync — kết quả là soft delete (set IsDeleted=true)
            // sau khi đã xóa hết children. Nếu muốn hard delete, gọi method mới đó.
            await _carRepo.DeleteCarAsync(id);

            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: null,
                roleTarget: AppRoles.Manager,
                title: "Cảnh báo: Dữ liệu xe bị xóa 🗑️",
                content: $"Admin hệ thống vừa xóa mẫu {car.Brand} {car.Name} cùng toàn bộ hình ảnh liên quan.",
                actionUrl: "/admin/cars",
                type: "SystemAlert"
            );

            return true;
        }

        // ==========================================================
        // SYNC STATUS THEO TỒN KHO
        // ==========================================================
        private async Task SyncCarStatusAsync(int carId)
        {
            int totalQuantity = await _inventoryRepo.GetTotalQuantityByCarIdAsync(carId);

            var car = await _carRepo.GetByIdAsync(carId);
            if (car == null) return;

            if (totalQuantity <= 0 && car.Status == CarStatus.Available)
            {
                car.Status = CarStatus.Out_of_stock;
                car.UpdatedAt = DateTime.UtcNow;
                await _carRepo.UpdateAsync(car);
            }
            else if (totalQuantity > 0 && car.Status == CarStatus.Out_of_stock)
            {
                car.Status = CarStatus.Available;
                car.UpdatedAt = DateTime.UtcNow;
                await _carRepo.UpdateAsync(car);
            }
        }

        // ==========================================================
        // CLONE CAR
        // ==========================================================
        public async Task<(bool Success, string Message, int? NewCarId)> CloneCarAsync(
            int id, string userRole, int? userShowroomId, int? createdByUserId = null)
        {
            var oldCar = await _carRepo.GetCarDetailForAdminAsync(id);
            if (oldCar == null) return (false, "Không tìm thấy con xe gốc để nhân bản!", null);

            if (userRole != "Admin" && userShowroomId.HasValue)
            {
                var inventories = await _inventoryRepo.GetInventoriesByCarIdAsync(id);
                if (!inventories.Any(i => i.ShowroomId == userShowroomId.Value))
                    return (false, "Ní không được phép nhân bản xe của showroom chi nhánh khác đâu nhé!", null);
            }

            // ✨ FIX: Build CarColors trước khi AddCarAsync để EF insert kèm
            var clonedColors = (oldCar.CarColors != null && oldCar.CarColors.Any())
                ? oldCar.CarColors.Select(c => new CarColor
                {
                    ColorName = c.ColorName,
                    HexCode = c.HexCode,
                    ImageUrl = c.ImageUrl,
                    IsActive = c.IsActive,
                    CreatedAt = DateTime.UtcNow
                }).ToList()
                : new List<CarColor>();

            var newCar = new Car
            {
                Name = oldCar.Name + " (Bản sao chưa duyệt)",
                Brand = oldCar.Brand,
                Model = oldCar.Model,
                Year = oldCar.Year,
                Price = oldCar.Price,
                Mileage = oldCar.Mileage,
                FuelType = oldCar.FuelType,
                Transmission = oldCar.Transmission,
                BodyStyle = oldCar.BodyStyle,
                Description = oldCar.Description,
                Condition = oldCar.Condition,
                Status = CarStatus.Draft,
                ImageUrl = "/uploads/Cars/default-car.png",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false,
                CreatedByUserId = createdByUserId,
                CarColors = clonedColors // ✨ FIX: gán trước khi Add
            };

            await _carRepo.AddCarAsync(newCar);

            // 4. Specs
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

            // 5. Features
            if (oldCar.CarFeatures != null && oldCar.CarFeatures.Any())
            {
                var newFeatures = oldCar.CarFeatures.Select(f => new CarFeature
                {
                    CarId = newCar.CarId,
                    FeatureId = f.FeatureId
                }).ToList();
                await _carFeatureRepo.AddRangeAsync(newFeatures);
            }

            // 6. Inventory
            int targetShowroomId = (userRole != "Admin" && userShowroomId.HasValue)
                ? userShowroomId.Value : 1;

            await _inventoryRepo.AddInventoryAsync(new CarInventory
            {
                CarId = newCar.CarId,
                ShowroomId = targetShowroomId,
                Quantity = 1,
                DisplayStatus = "Pending",
                UpdatedAt = DateTime.UtcNow
            });

            return (true, "Đã nhân bản bản nháp thành công!", newCar.CarId);
        }

        // ==========================================================
        // SEARCH MASTER MODELS
        // ==========================================================
        public async Task<IEnumerable<object>> SearchMasterModelsAsync(string query)
        {
            var cars = await _carRepo.SearchMasterCarsAsync(query);
            return cars.Select(c => new
            {
                c.CarId,
                c.Name,
                c.Brand,
                c.Year,
                c.ImageUrl,
                DisplayTitle = $"{c.Brand} {c.Name} ({c.Year})"
            });
        }

        // ==========================================================
        // 11. APPROVE / 12. REJECT / 13. CHANGE STATUS / BULK
        // ==========================================================
        public async Task<(bool Success, string Message)> ApproveCarAsync(int carId, string userRole, int? userShowroomId)
        {
            var car = await _carRepo.GetByIdAsync(carId);
            if (car == null) return (false, "Không tìm thấy xe!");

            if (car.Status == CarStatus.Available)
                return (false, "Xe này đang bán rồi, duyệt gì nữa ní!");

            if ((userRole == "ShowroomManager" || userRole == "SalesManager") && userShowroomId.HasValue)
            {
                var inventories = await _inventoryRepo.GetInventoriesByCarIdAsync(carId);
                if (!inventories.Any(i => i.ShowroomId == userShowroomId.Value))
                {
                    return (false, "Sếp ơi, xe này thuộc chi nhánh khác, sếp không có thẩm quyền duyệt đâu!");
                }
            }

            car.Status = CarStatus.Available;
            car.RejectionReason = null;
            car.UpdatedAt = DateTime.UtcNow;
            await _carRepo.UpdateAsync(car);

            var showroomId = (await _inventoryRepo.GetInventoriesByCarIdAsync(carId)).FirstOrDefault()?.ShowroomId;
            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: showroomId,
                roleTarget: $"{AppRoles.Sales},{AppRoles.ShowroomSales},{AppRoles.Marketing}",
                title: "Xe đã lên sàn! 🎉",
                content: $"Sếp đã duyệt mẫu {car.Brand} {car.Name}.",
                actionUrl: $"/admin/cars/detail/{carId}",
                type: "CarApproval"
            );
            return (true, "Đã duyệt xe thành công! Xe đã lên sóng.");
        }

        public async Task<(bool Success, string Message)> RejectCarAsync(int carId, string reason)
        {
            var car = await _carRepo.GetByIdAsync(carId);
            if (car == null) return (false, "Không tìm thấy xe!");

            if (string.IsNullOrWhiteSpace(reason))
                return (false, "Từ chối thì phải ghi rõ lý do cho lính nó biết đường sửa chứ sếp!");

            car.Status = CarStatus.Rejected;
            car.RejectionReason = reason;
            car.UpdatedAt = DateTime.UtcNow;
            await _carRepo.UpdateAsync(car);

            var showroomId = (await _inventoryRepo.GetInventoriesByCarIdAsync(carId)).FirstOrDefault()?.ShowroomId;
            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: showroomId,
                roleTarget: $"{AppRoles.Sales},{AppRoles.ShowroomSales},{AppRoles.Content}",
                title: "Xe bị từ chối duyệt 🚨",
                content: $"Mẫu {car.Brand} {car.Name} bị sếp chê. Lý do: {reason}.",
                actionUrl: $"/admin/cars/edit/{carId}",
                type: "CarApproval"
            );
            return (true, "Đã từ chối và gửi phản hồi lại cho nhân viên!");
        }

        public async Task<(bool Success, string Message)> ChangeCarStatusAsync(int carId, CarStatus newStatus)
        {
            var car = await _carRepo.GetByIdAsync(carId);
            if (car == null) return (false, "Không tìm thấy xe!");

            car.Status = newStatus;
            car.UpdatedAt = DateTime.UtcNow;
            car.RejectionReason = null;
            await _carRepo.UpdateAsync(car);

            var showroomId = (await _inventoryRepo.GetInventoriesByCarIdAsync(carId)).FirstOrDefault()?.ShowroomId;
            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: showroomId,
                roleTarget: $"{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales}",
                title: "Trạng thái xe bị thay đổi đột ngột 🔄",
                content: $"Mẫu {car.Brand} {car.Name} vừa bị quản trị viên đổi trạng thái thành {newStatus}.",
                actionUrl: $"/admin/cars/detail/{carId}",
                type: "CarStatus"
            );
            return (true, $"Đã ép trạng thái xe thành: {newStatus}");
        }

        public async Task<bool> BulkApproveAsync(List<int> carIds)
        {
            foreach (var id in carIds)
            {
                var car = await _carRepo.GetByIdAsync(id);
                if (car != null && car.Status == CarStatus.PendingApproval)
                {
                    car.Status = CarStatus.Available;
                    car.UpdatedAt = DateTime.UtcNow;
                    await _carRepo.UpdateAsync(car);
                }
            }
            return true;
        }
    }
}