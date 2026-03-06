using Microsoft.AspNetCore.Http;
using CoreEntities.Models;

namespace LogicBusiness.DTOs
{
    public class CarCreateDto
    {
        // --- CÁC THÔNG SỐ CƠ BẢN CỦA XE ---
        public string Name { get; set; } = null!;
        public string? Brand { get; set; }
        public string? Model { get; set; }       // Bổ sung
        public string? Color { get; set; }       // Bổ sung
        public decimal Price { get; set; }
        public string? FuelType { get; set; }    // Bổ sung
        public decimal? Mileage { get; set; }    // Bổ sung
        public string? Description { get; set; } // Bổ sung
        public int Year { get; set; }

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
    //public class SpecCategoryDto
    //{
    //    public string Category { get; set; } = null!;
    //    public List<SpecDetailDto> Items { get; set; } = new List<SpecDetailDto>();
    //}
    //public class SpecDetailDto
    //{
    //    public string Name { get; set; } = null!;
    //    public string Value { get; set; } = null!;
    //}

    //public class SpecCategoryDto
    //{
    //    public string Category { get; set; }
    //    public List<SpecItemDto> Items { get; set; }
    //}

    //public class SpecItemDto
    //{
    //    public string Name { get; set; }  // Phải là Name
    //    public string Value { get; set; } // Phải là Value
    //}

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
