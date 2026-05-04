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
    public class CarCreateDto
    {
        [Required(ErrorMessage = "Tên xe không được để trống!")]
        [StringLength(255, ErrorMessage = "Tên xe quá dài, tối đa 255 ký tự thôi!")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Vui lòng nhập tên hãng xe!")]
        public string Brand { get; set; } = null!;
        public string? Model { get; set; }
        public string? Color { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Giá xe phải lớn hơn hoặc bằng 0!")]
        public decimal Price { get; set; }
        [RegularExpression("^(Xăng|Điện|Dầu|Hybrid)$", ErrorMessage = "Nhiên liệu chỉ được nhập Xăng, Điện, Dầu hoặc Hybrid!")]
        public string? FuelType { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Số Km (ODO) không được âm!")]
        public double? Mileage { get; set; }
        public string? Description { get; set; }
        [ValidYearAttribute]
        public int Year { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng xe không được nhỏ hơn 0!")]
        public int Quantity { get; set; }
        [RegularExpression("^(Số sàn|Số tự động)$", ErrorMessage = "Hộp số chỉ được nhập 'Số sàn' hoặc 'Số tự động'")]
        public string? Transmission { get; set; }
        public int ShowroomId { get; set; }
        [RegularExpression("^(Sedan|SUV|Hatchback|Crossover|MPV|Bán tải|Coupe)$", ErrorMessage = "Kiểu dáng xe không hợp lệ. Vui lòng chọn đúng danh mục!")]
        public string? BodyStyle { get; set; }
        public CarCondition Condition { get; set; }
        public CarStatus? Status { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? FeatureIds { get; set; } = "1, 2, 3";
        public string? Specifications { get; set; } = "Động cơ|Mã lực|300 HP;Kích thước|Chiều dài|4940 mm";
    }

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
        public string? Color { get; set; }
        public string? FuelType { get; set; }
        public double? Mileage { get; set; }
        public string? Description { get; set; }
        public string? Transmission { get; set; }
        public string? BodyStyle { get; set; }
        public CarCondition Condition { get; set; }
        public CarStatus? Status { get; set; }
        public IFormFile? ImageFile { get; set; }
        public decimal? Price { get; set; }
        public string? FeatureIds { get; set; }
        public string? SpecificationsJson { get; set; }
        public string? Specifications { get; set; }
        public string? PricingVersionsJson { get; set; }
        // JSON array: [{ "showroomId":1, "quantity":3, "displayStatus":"OnDisplay", "color":"Đỏ" }, ...]
        public string? InventoriesJson { get; set; }
        public int ShowroomId { get; set; }
        public int Quantity { get; set; }
        public List<IFormFile>? GalleryFiles { get; set; }
        public string? GalleryMetasJson { get; set; }
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
        public string Category { get; set; } = null!;
        public string SpecName { get; set; } = null!;
        public string SpecValue { get; set; } = null!;
    }

    public class CarPricingVersionCreateDto
    {
        public string VersionName { get; set; } = null!;
        public decimal PriceVnd { get; set; }
        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }

    public class CarInventoryCreateDto
    {
        public int ShowroomId { get; set; }
        public int Quantity { get; set; }
        public string? DisplayStatus { get; set; }
        // Màu xe cụ thể của lô hàng này (nullable — kho cũ không có màu)
        public string? Color { get; set; }
    }

    // UpdateStockDto nằm ở UpdateStockDto.cs riêng — không define ở đây tránh ambiguity

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

    public class CarUpdateDto
    {
        [Required(ErrorMessage = "Tên xe không được để trống!")]
        [StringLength(255, ErrorMessage = "Tên xe quá dài, tối đa 255 ký tự thôi!")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Vui lòng nhập tên hãng xe!")]
        public string Brand { get; set; } = null!;
        public string? Model { get; set; }
        public string? Color { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Giá xe phải lớn hơn hoặc bằng 0!")]
        public decimal Price { get; set; }
        [RegularExpression("^(Xăng|Điện|Dầu|Hybrid)$", ErrorMessage = "Nhiên liệu chỉ được nhập Xăng, Điện, Dầu hoặc Hybrid!")]
        public string? FuelType { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Số Km (ODO) không được âm!")]
        public double? Mileage { get; set; }
        public string? Description { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng xe không được nhỏ hơn 0!")]
        public int ShowroomId { get; set; }
        public int Quantity { get; set; }
        [RegularExpression("^(Số sàn|Số tự động)$", ErrorMessage = "Hộp số chỉ được nhập 'Số sàn' hoặc 'Số tự động'")]
        public string? Transmission { get; set; }
        [RegularExpression("^(Sedan|SUV|Hatchback|Crossover|MPV|Bán tải|Coupe)$", ErrorMessage = "Kiểu dáng xe không hợp lệ. Vui lòng chọn đúng danh mục!")]
        public string? BodyStyle { get; set; }
        [ValidYearAttribute]
        public int Year { get; set; }
        public int Condition { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? FeatureIds { get; set; }
        public string? Specifications { get; set; }
        public CarStatus? Status { get; set; }
    }

    public class CarFilterDto
    {
        public int PageNumber { get; set; } = 1;
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

    public class CarSpecificationDto
    {
        public string Category { get; set; } = null!;
        public string SpecName { get; set; } = null!;
        public string SpecValue { get; set; } = null!;
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