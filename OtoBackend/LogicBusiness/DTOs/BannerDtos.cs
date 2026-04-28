using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; // Bắt buộc phải có using này để dùng được IFormFile

namespace LogicBusiness.DTOs
{
    // 1. DTO chuyên dùng để nhận File từ Frontend (Fix lỗi Swagger 500)
    public class BannerUploadRequest
    {
        [Required(ErrorMessage = "Vui lòng chọn file ảnh.")]
        public IFormFile File { get; set; } = null!;

        public string? FolderName { get; set; }
    }

    // 2. DTO dùng khi Admin bấm "Tạo mới"
    public class BannerCreateDto
    {
        [Required(ErrorMessage = "Tên banner không được để trống")]
        [StringLength(200, ErrorMessage = "Tên banner không được vượt quá 200 ký tự")]
        public string BannerName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng upload ảnh trước khi lưu")]
        public string ImageUrl { get; set; } = null!;

        public string? LinkUrl { get; set; }

        public int Position { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }

    // 3. DTO dùng khi Admin bấm "Cập nhật"
    public class BannerUpdateDto
    {
        [Required(ErrorMessage = "Tên banner không được để trống")]
        public string BannerName { get; set; } = null!;

        // Đã bỏ [Required] ở đây. Rất quan trọng!
        // Giúp nhân viên có thể sửa Tên/Link mà KHÔNG cần phải upload lại ảnh cũ
        public string? ImageUrl { get; set; }

        public string? LinkUrl { get; set; }

        public int Position { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}