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

        // --- CẤU TRÚC ĐỊA CHỈ MỚI ---
        public string Province { get; set; } = null!;
        public string District { get; set; } = null!;
        public string StreetAddress { get; set; } = null!;
        public string? FullAddress { get; set; } // Gom lại sẵn cho FE lười ghép chuỗi

        public string? Hotline { get; set; }
    }

    // DTO để Admin tạo mới Showroom
    public class ShowroomCreateDto
    {
        public string Name { get; set; } = null!;

        // Buộc Admin phải nhập đủ 3 phần này
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
    public class ShowroomCarResponseDto
    {
        public int CarId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string? MainImageUrl { get; set; }
        public int Quantity { get; set; }
        public string DisplayStatus { get; set; } = null!;
    }
}