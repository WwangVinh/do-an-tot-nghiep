using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    public class PromotionAdminDto
    {
        public int PromotionId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public int? CarId { get; set; }
        public string? CarName { get; set; } // Hiển thị tên xe cho Admin dễ nhìn
        public int? MaxUsage { get; set; }
        public int CurrentUsage { get; set; }
    }

    public class PromotionCreateUpdateDto
    {
        [Required(ErrorMessage = "Vui lòng nhập mã giảm giá")]
        public string Code { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(0, 100, ErrorMessage = "Phần trăm giảm chỉ từ 0 đến 100")]
        public decimal? DiscountPercentage { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = "Active";

        public int? CarId { get; set; }
        public int? MaxUsage { get; set; }
    }
}
