using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LogicBusiness.DTOs
{
    // ═══════════════════════════════════════════════════════════════
    // KHÁCH HÀNG — Gửi yêu cầu ký gửi xe
    // ═══════════════════════════════════════════════════════════════
    public class ConsignmentCreateDto
    {
        // --- Thông tin khách hàng ---

        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        [MaxLength(255, ErrorMessage = "Họ tên không được vượt quá 255 ký tự.")]
        public string GuestName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [MaxLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string GuestPhone { get; set; } = null!;

        [MaxLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string? GuestEmail { get; set; }

        // --- Thông tin xe ---

        [Required(ErrorMessage = "Vui lòng nhập hãng xe.")]
        [MaxLength(100, ErrorMessage = "Hãng xe không được vượt quá 100 ký tự.")]
        public string Brand { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập dòng xe.")]
        [MaxLength(100, ErrorMessage = "Dòng xe không được vượt quá 100 ký tự.")]
        public string Model { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập năm sản xuất.")]
        [Range(1900, 2100, ErrorMessage = "Năm sản xuất phải nằm trong khoảng 1900 – 2100.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số km đã đi.")]
        [Range(0, double.MaxValue, ErrorMessage = "Số km không được âm.")]
        public decimal Mileage { get; set; }

        [MaxLength(1000, ErrorMessage = "Mô tả tình trạng xe không được vượt quá 1000 ký tự.")]
        public string? ConditionDescription { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá kỳ vọng.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá kỳ vọng không được âm.")]
        public decimal ExpectedPrice { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════
    // ADMIN / MANAGER — Duyệt, thẩm định, chốt giá hồ sơ ký gửi
    // ═══════════════════════════════════════════════════════════════
    public class ConsignmentUpdateDto
    {
        [Required(ErrorMessage = "Vui lòng cung cấp trạng thái cập nhật.")]
        public string Status { get; set; } = null!;
        // Các giá trị hợp lệ: Pending | Appraising | Approved | Rejected | Completed

        [Range(0, double.MaxValue, ErrorMessage = "Giá thỏa thuận không được âm.")]
        public decimal? AgreedPrice { get; set; }

        [Range(0, 100, ErrorMessage = "Tỷ lệ hoa hồng phải nằm trong khoảng 0% – 100%.")]
        public decimal? CommissionRate { get; set; }

        public int? LinkedCarId { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════
    // RESPONSE — Chi tiết hồ sơ (dành cho admin xem)
    // ═══════════════════════════════════════════════════════════════
    public class ConsignmentResponseDto
    {
        public int ConsignmentId { get; set; }
        public string GuestName { get; set; } = null!;
        public string GuestPhone { get; set; } = null!;
        public string? GuestEmail { get; set; }
        public string Brand { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year { get; set; }
        public decimal Mileage { get; set; }
        public string? ConditionDescription { get; set; }
        public decimal ExpectedPrice { get; set; }
        public decimal? AgreedPrice { get; set; }
        public decimal? CommissionRate { get; set; }
        public string Status { get; set; } = null!;
        public int? LinkedCarId { get; set; }
        public string CreatedAt { get; set; } = null!;
        public string? UpdatedAt { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════
    // LIST ITEM — Dùng cho danh sách admin (nhẹ hơn ResponseDto)
    // ═══════════════════════════════════════════════════════════════
    public class ConsignmentListItemDto
    {
        public int ConsignmentId { get; set; }
        public string GuestName { get; set; } = null!;
        public string GuestPhone { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year { get; set; }
        public decimal ExpectedPrice { get; set; }
        public string Status { get; set; } = null!;
        public string CreatedAt { get; set; } = null!;
    }

    // ═══════════════════════════════════════════════════════════════
    // CONSTANTS — Tập trung các giá trị Status hợp lệ
    // ═══════════════════════════════════════════════════════════════
    public static class ConsignmentStatus
    {
        public const string Pending = "Pending";
        public const string Appraising = "Appraising";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public const string Completed = "Completed";

        public static readonly IReadOnlyList<string> All = new[]
        {
            Pending, Appraising, Approved, Rejected, Completed
        };

        public static bool IsValid(string status) => All.Contains(status);

        public static string ToVietnamese(string status) => status switch
        {
            Pending => "đang chờ xử lý",
            Appraising => "đang được thẩm định",
            Approved => "đã được phê duyệt",
            Rejected => "bị từ chối",
            Completed => "đã hoàn thành",
            _ => status
        };
    }
}