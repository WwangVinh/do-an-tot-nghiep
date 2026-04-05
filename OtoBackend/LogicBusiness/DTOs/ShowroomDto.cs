using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    // DTO để trả dữ liệu về cho Frontend (Xem danh sách/Chi tiết)
    public class ShowroomDto
    {
        public int ShowroomId { get; set; }
        public string Name { get; set; } = null!;

        public string Province { get; set; } = null!;
        public string District { get; set; } = null!;
        public string StreetAddress { get; set; } = null!;
        public string? FullAddress { get; set; } // Gom lại sẵn cho FE

        public string? Hotline { get; set; }
    }

    // DTO để Admin tạo mới Showroom
    public class ShowroomCreateDto
    {
        public string Name { get; set; } = null!;
        public string Province { get; set; } = null!;
        public string District { get; set; } = null!;
        public string StreetAddress { get; set; } = null!;

        public string? Hotline { get; set; }
    }

    // DTO để Admin cập nhật Showroom
    public class ShowroomUpdateDto
    {
        public string Name { get; set; } = null!;

        public string Province { get; set; } = null!;
        public string District { get; set; } = null!;
        public string StreetAddress { get; set; } = null!;

        public string? Hotline { get; set; }
    }

    // DTO để trả về thông tin xe đang có tại Showroom (Dùng cho cả danh sách & chi tiết)
    public class ShowroomCarResponseDto
    {
        //Thông tin định danh cơ bản
        public int CarId { get; set; }
        public string Name { get; set; } = null!; // Tên xe đầy đủ (VD: VinFast VF8 Plus)
        public decimal Price { get; set; }
        public string? MainImageUrl { get; set; }

        // Thông tin chi tiết để hiển thị & lọc (SỬA Ở ĐÂY)
        public string? BrandName { get; set; }       // Hãng xe (VinFast)
        public string? SegmentName { get; set; }     // Phân khúc (SUV-C)
        public string? FuelTypeName { get; set; }    // Nhiên liệu (Điện)
        public string? TransmissionName { get; set; } // Hộp số (Tự động)
        public string? ColorName { get; set; }        // Màu sắc (Đỏ)
        public int? ModelYear { get; set; }           // Năm sản xuất (2023)
        public string? Origin { get; set; }          // Xuất xứ (Trong nước)

        // 3. Thông tin tồn kho tại Showroom này
        public int Quantity { get; set; }
        public string DisplayStatus { get; set; } = null!; // Available, Out of stock
    }
}