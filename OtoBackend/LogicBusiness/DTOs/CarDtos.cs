using CoreEntities.Models;
using Microsoft.AspNetCore.Http;
using LogicBusiness.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    // ==========================================================
    // CREATE DTOs
    // ==========================================================

    /// <summary>
    /// DTO tạo xe đơn giản — không có Gallery, PricingVersion.
    /// Dùng khi thêm xe nhanh từ form admin cơ bản.
    /// </summary>
    public class CarCreateDto
    {
        [Required(ErrorMessage = "Tên xe không được để trống!")]
        [StringLength(255, ErrorMessage = "Tên xe quá dài, tối đa 255 ký tự thôi!")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập tên hãng xe!")]
        public string Brand { get; set; } = null!;

        public string? Model { get; set; }

        /// <summary>
        /// JSON array các màu xe.
        /// Format: [{"colorName":"Đỏ","hexCode":"#FF0000","imageUrl":"https://..."}]
        /// Để null/empty nếu xe không có cấu hình màu.
        /// </summary>
        public string? ColorsJson { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá xe phải lớn hơn hoặc bằng 0!")]
        public decimal Price { get; set; }

        [RegularExpression("^(Xăng|Điện|Dầu|Hybrid)$",
            ErrorMessage = "Nhiên liệu chỉ được nhập Xăng, Điện, Dầu hoặc Hybrid!")]
        public string? FuelType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Số Km (ODO) không được âm!")]
        public double? Mileage { get; set; }

        public string? Description { get; set; }

        [ValidYearAttribute]
        public int Year { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng xe không được nhỏ hơn 0!")]
        public int Quantity { get; set; }

        [RegularExpression("^(Số sàn|Số tự động)$",
            ErrorMessage = "Hộp số chỉ được nhập 'Số sàn' hoặc 'Số tự động'")]
        public string? Transmission { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn showroom!")]
        public int ShowroomId { get; set; }

        [RegularExpression("^(Sedan|SUV|Hatchback|Crossover|MPV|Bán tải|Coupe)$",
            ErrorMessage = "Kiểu dáng xe không hợp lệ. Vui lòng chọn đúng danh mục!")]
        public string? BodyStyle { get; set; }

        public CarCondition Condition { get; set; }
        public CarStatus? Status { get; set; }
        public IFormFile? ImageFile { get; set; }

        // ⚠️ FIX: BỎ default value "1, 2, 3" — đây là dữ liệu test, sẽ bị leak vào prod
        // nếu FE quên truyền. FE phải truyền explicitly.
        /// <summary>
        /// Comma-separated feature IDs. Ví dụ: "1,2,5,8"
        /// </summary>
        public string? FeatureIds { get; set; }

        // ⚠️ FIX: BỎ default value mẫu — bug y chang FeatureIds
        /// <summary>
        /// Format: "Category|SpecName|SpecValue;Category|SpecName|SpecValue"
        /// Ví dụ: "Động cơ|Mã lực|300 HP;Kích thước|Chiều dài|4940 mm"
        /// </summary>
        public string? Specifications { get; set; }
    }

    /// <summary>
    /// DTO tạo xe đầy đủ — có Gallery, PricingVersion, Inventories chi tiết theo màu.
    /// </summary>
    public class CarCreateFullDto
    {
        [Required(ErrorMessage = "Tên xe không được để trống!")]
        [StringLength(255, ErrorMessage = "Tên xe quá dài, tối đa 255 ký tự thôi!")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập tên hãng xe!")]
        public string Brand { get; set; } = null!;

        [ValidYearAttribute]
        public int Year { get; set; }

        public string? Model { get; set; }

        /// <summary>
        /// JSON array các màu xe.
        /// Format: [{"colorName":"Đỏ","hexCode":"#FF0000","imageUrl":"https://..."}]
        /// </summary>
        public string? ColorsJson { get; set; }

        [RegularExpression("^(Xăng|Điện|Dầu|Hybrid)$",
            ErrorMessage = "Nhiên liệu chỉ được nhập Xăng, Điện, Dầu hoặc Hybrid!")]
        public string? FuelType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Số Km (ODO) không được âm!")]
        public double? Mileage { get; set; }

        public string? Description { get; set; }

        [RegularExpression("^(Số sàn|Số tự động)$",
            ErrorMessage = "Hộp số chỉ được nhập 'Số sàn' hoặc 'Số tự động'")]
        public string? Transmission { get; set; }

        [RegularExpression("^(Sedan|SUV|Hatchback|Crossover|MPV|Bán tải|Coupe)$",
            ErrorMessage = "Kiểu dáng xe không hợp lệ!")]
        public string? BodyStyle { get; set; }

        public CarCondition Condition { get; set; }
        public CarStatus? Status { get; set; }
        public IFormFile? ImageFile { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá xe phải lớn hơn hoặc bằng 0!")]
        public decimal? Price { get; set; }

        public string? FeatureIds { get; set; }
        public string? SpecificationsJson { get; set; }
        public string? Specifications { get; set; }
        public string? PricingVersionsJson { get; set; }

        /// <summary>
        /// JSON array kho theo màu cho từng showroom.
        /// Format: [{"showroomId":1,"quantity":3,"displayStatus":"OnDisplay","carColorId":12}, ...]
        /// CarColorId phải khớp với 1 màu trong ColorsJson (validate ở Service).
        /// </summary>
        public string? InventoriesJson { get; set; }

        public int ShowroomId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng xe không được nhỏ hơn 0!")]
        public int Quantity { get; set; }

        public List<IFormFile>? GalleryFiles { get; set; }

        /// <summary>
        /// JSON array meta cho mỗi gallery file (số phần tử phải bằng GalleryFiles.Count).
        /// Format: [{"title":"...","description":"...","imageType":"Exterior","isMainImage":false}]
        /// </summary>
        public string? GalleryMetasJson { get; set; }
    }

    // ==========================================================
    // UPDATE DTOs
    // ==========================================================

    public class CarUpdateDto
    {
        [Required(ErrorMessage = "Tên xe không được để trống!")]
        [StringLength(255, ErrorMessage = "Tên xe quá dài, tối đa 255 ký tự thôi!")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập tên hãng xe!")]
        public string Brand { get; set; } = null!;

        public string? Model { get; set; }

        /// <summary>
        /// JSON array các màu xe (REPLACE-ALL semantic).
        /// Format: [{"colorName":"Đỏ","hexCode":"#FF0000","imageUrl":"..."}]
        /// - null/empty → giữ nguyên màu cũ
        /// - "[]" → xóa hết màu
        /// - có items → thay thế toàn bộ
        /// </summary>
        public string? ColorsJson { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá xe phải lớn hơn hoặc bằng 0!")]
        public decimal Price { get; set; }

        [RegularExpression("^(Xăng|Điện|Dầu|Hybrid)$",
            ErrorMessage = "Nhiên liệu chỉ được nhập Xăng, Điện, Dầu hoặc Hybrid!")]
        public string? FuelType { get; set; }

        // ⚠️ FIX: Mileage giờ non-nullable (default 0) thay vì double?
        // Trước đây service cast (decimal)dto.Mileage có thể NRE nếu null.
        [Range(0, double.MaxValue, ErrorMessage = "Số Km (ODO) không được âm!")]
        public double Mileage { get; set; } = 0;

        public string? Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn showroom!")]
        public int ShowroomId { get; set; }

        // ⚠️ FIX: Thêm validation cho Quantity
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng xe không được nhỏ hơn 0!")]
        public int Quantity { get; set; }

        [RegularExpression("^(Số sàn|Số tự động)$",
            ErrorMessage = "Hộp số chỉ được nhập 'Số sàn' hoặc 'Số tự động'")]
        public string? Transmission { get; set; }

        [RegularExpression("^(Sedan|SUV|Hatchback|Crossover|MPV|Bán tải|Coupe)$",
            ErrorMessage = "Kiểu dáng xe không hợp lệ. Vui lòng chọn đúng danh mục!")]
        public string? BodyStyle { get; set; }

        [ValidYearAttribute]
        public int Year { get; set; }

        // ⚠️ FIX QUAN TRỌNG: Đổi từ int sang CarCondition enum.
        // Lý do: tránh mapping sai khi FE truyền giá trị enum không hợp lệ.
        // Service đang cast (CarCondition)dto.Condition rất nguy hiểm với int tự do.
        public CarCondition Condition { get; set; }

        public IFormFile? ImageFile { get; set; }
        public string? FeatureIds { get; set; }
        public string? Specifications { get; set; }
        public CarStatus? Status { get; set; }
    }

    // ==========================================================
    // SUB-ENTITY CREATE DTOs
    // ==========================================================

    public class CarColorCreateDto
    {
        [Required(ErrorMessage = "Tên màu không được để trống!")]
        [StringLength(100, ErrorMessage = "Tên màu tối đa 100 ký tự!")]
        public string ColorName { get; set; } = null!;

        [StringLength(20, ErrorMessage = "Mã hex tối đa 20 ký tự!")]
        [RegularExpression(@"^#?[0-9A-Fa-f]{3,8}$",
            ErrorMessage = "HexCode không hợp lệ. Ví dụ đúng: #FF0000")]
        public string? HexCode { get; set; }

        public string? ImageUrl { get; set; }
    }

    public class CarImageMetaDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImageType { get; set; }
        public bool? IsMainImage { get; set; }
    }

    public class CarSpecificationCreateDto
    {
        [Required(ErrorMessage = "Category không được để trống!")]
        public string Category { get; set; } = null!;

        [Required(ErrorMessage = "SpecName không được để trống!")]
        public string SpecName { get; set; } = null!;

        [Required(ErrorMessage = "SpecValue không được để trống!")]
        public string SpecValue { get; set; } = null!;
    }

    public class CarPricingVersionCreateDto
    {
        [Required(ErrorMessage = "Tên phiên bản giá không được để trống!")]
        public string VersionName { get; set; } = null!;

        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0!")]
        public decimal PriceVnd { get; set; }

        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }

    public class CarInventoryCreateDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "ShowroomId không hợp lệ!")]
        public int ShowroomId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không được âm!")]
        public int Quantity { get; set; }

        public string? DisplayStatus { get; set; }

        /// <summary>
        /// Màu xe cụ thể của lô hàng này.
        /// Nullable để backward compat với kho cũ chưa có màu.
        /// Validate ở Service: phải thuộc về CarColors của xe.
        /// </summary>
        public int? CarColorId { get; set; }
    }

    // ==========================================================
    // QUERY / FILTER DTOs
    // ==========================================================

    public class CarFilterDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "PageNumber phải >= 1!")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "PageSize phải từ 1 đến 100!")]
        public int PageSize { get; set; } = 10;

        public string? Transmission { get; set; }
        public string? BodyStyle { get; set; }
        public string? Keyword { get; set; }
        public string? Brand { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? Condition { get; set; }
        public int? Status { get; set; }
    }

    // ==========================================================
    // RESPONSE / READ DTOs
    // ==========================================================

    public class CarSpecificationDto
    {
        public string Category { get; set; } = null!;
        public string SpecName { get; set; } = null!;
        public string SpecValue { get; set; } = null!;
    }

    public class SpecCategoryDto
    {
        public string Category { get; set; } = null!;
        public List<SpecDetailDto> Items { get; set; } = new List<SpecDetailDto>();
    }

    public class SpecDetailDto
    {
        public string Name { get; set; } = null!;
        public string Value { get; set; } = null!;
    }

    public class UpdateImageDetailsDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
    }

    public class ChangeStatusRequestDto
    {
        public CarStatus NewStatus { get; set; }
    }

}