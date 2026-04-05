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

    /// DTO dùng để NHẬN dữ liệu khi Thêm Xe Mới.
    /// Bao gồm đầy đủ các ràng buộc Validation (Required, Range, Regex) và hỗ trợ Upload file ảnh.
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

        // --- CÁC TRƯỜNG PHỤ TRỢ ---
        public IFormFile? ImageFile { get; set; }

        public string? FeatureIds { get; set; } = "1, 2, 3";

        public string? Specifications { get; set; } = "Động cơ|Mã lực|300 HP;Kích thước|Chiều dài|4940 mm";

    }

    /// DTO bổ trợ để cấu trúc hóa dữ liệu Thông số kỹ thuật (Specifications) theo nhóm.
    /// Thường dùng để parse từ chuỗi String sang Object để hiển thị lên giao diện.
    public class SpecCategoryDto
    {
        public string Category { get; set; } = null!;
        public List<SpecDetailDto> Items { get; set; } = new List<SpecDetailDto>();
    }

    /// DTO chi tiết cho từng dòng thông số (Tên và Giá trị).
    /// Là một phần tử nằm trong danh sách Items của SpecCategoryDto.
    public class SpecDetailDto
    {
        public string Name { get; set; } = null!;  // Phải là Name
        public string Value { get; set; } = null!; // Phải là Value
    }

    /// DTO dùng để NHẬN dữ liệu khi Cập Nhật thông tin xe.
    /// Tương tự CreateDto nhưng các trường thường linh hoạt hơn và xử lý ghi đè ảnh/thông số cũ.
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

    /// DTO chứa các tham số Bộ Lọc và Phân Trang.
    /// Được FE gửi lên qua Query String để tìm kiếm xe theo giá, hãng, tình trạng, v.v.
    public class CarFilterDto
    {
        // Phân trang mặc định là trang 1, mỗi trang 10 xe
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

    /// DTO đại diện cho một dòng thông số kỹ thuật đơn lẻ.
    /// Thường dùng làm trung gian để map dữ liệu từ Database ra Client.
    public class CarSpecificationDto
    {
        public string Category { get; set; } = null!;  // Ví dụ: Động cơ
        public string SpecName { get; set; } = null!;  // Ví dụ: Loại động cơ
        public string SpecValue { get; set; } = null!; // Ví dụ: 1.5L Turbo
    }

    /// DTO dùng để cập nhật lại Tiêu đề hoặc Mô tả cho một tấm ảnh xe đã upload.
    public class UpdateImageDetailsDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
    }

    /// DTO chỉ dùng để thay đổi trạng thái của xe (Ví dụ: Từ Nháp -> Hiển thị, hoặc Ẩn xe).
    public class ChangeStatusRequestDto
    {
        public CarStatus NewStatus { get; set; }
    }
}
