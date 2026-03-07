using CoreEntities.Models;
using Microsoft.AspNetCore.Http;
using OtoBackend.Helpers;
using System.ComponentModel.DataAnnotations;

namespace LogicBusiness.DTOs
{
    public class CarCreateDto
    {
        // --- CÁC THÔNG SỐ CƠ BẢN CỦA XE ---
        [Required(ErrorMessage = "Tên xe không được để trống!")]
        [StringLength(255, ErrorMessage = "Tên xe quá dài, tối đa 255 ký tự thôi!")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Vui lòng nhập tên hãng xe!")]
        public string Brand { get; set; } = null!;
        public string? Model { get; set; }       // Bổ sung
        public string? Color { get; set; }       // Bổ sung
        [Range(0, double.MaxValue, ErrorMessage = "Giá xe phải lớn hơn hoặc bằng 0!")]
        public decimal Price { get; set; }
        [RegularExpression("^(Xăng|Điện|Dầu|Hybrid)$", ErrorMessage = "Nhiên liệu chỉ được nhập Xăng, Điện, Dầu hoặc Hybrid!")]
        public string? FuelType { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Số Km (ODO) không được âm!")]
        public double? Mileage { get; set; }
        public string? Description { get; set; } // Bổ sung
        [ValidYearAttribute] // 👈 Tự động kiểm tra từ 1990 đến năm hiện tại!
        public int Year { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng xe không được nhỏ hơn 0!")]
        public int Quantity { get; set; }

        [RegularExpression("^(Số sàn|Số tự động)$", ErrorMessage = "Hộp số chỉ được nhập 'Số sàn' hoặc 'Số tự động'")]
        public string? Transmission { get; set; }


        [RegularExpression("^(Sedan|SUV|Hatchback|Crossover|MPV|Bán tải|Coupe)$", ErrorMessage = "Kiểu dáng xe không hợp lệ. Vui lòng chọn đúng danh mục!")]
        public string? BodyStyle { get; set; }

        public CarCondition Condition { get; set; } // Nếu xe có tình trạng (Mới/Cũ) thì mở comment dòng này

        // --- CÁC TRƯỜNG PHỤ TRỢ ---
        public IFormFile? ImageFile { get; set; }

        // Đổi từ List<int> sang string và gán mặc định luôn
        public string? FeatureIds { get; set; } = "1, 2, 3";

        // Thay vì để trống, mình nhét luôn 1 cục JSON mẫu làm mặc định
        // Thay vì để trống, mình nhét luôn 1 cục JSON mẫu làm mặc định
        // Trong file DTOs/CarCreateDto.cs
        public string? Specifications { get; set; } = "Động cơ|Mã lực|300 HP;Kích thước|Chiều dài|4940 mm";

    }

    public class SpecCategoryDto
    {
        public string Category { get; set; } = null!;
        public List<SpecDetailDto> Items { get; set; } = new List<SpecDetailDto>();
    }

    public class SpecDetailDto
    {
        public string Name { get; set; } = null!;  // Phải là Name
        public string Value { get; set; } = null!; // Phải là Value
    }

}
