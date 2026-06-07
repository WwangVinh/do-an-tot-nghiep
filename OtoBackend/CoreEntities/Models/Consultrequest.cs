using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntities.Models
{
    public class ConsultRequest
    {
        public int ConsultRequestId { get; set; }
        public int CarId { get; set; }
        public int ShowroomId { get; set; }

        public string CustomerName { get; set; } = null!;
        public string Phone { get; set; } = null!;

        // "Quotation" hoặc "Installment"
        public string RequestType { get; set; } = null!;

        // Ghi chú từ khách (nhu cầu, đời xe muốn, màu...)
        public string? CustomerNote { get; set; }

        // ===== Field dành riêng cho RequestType = "Installment" =====
        public decimal? MonthlyIncome { get; set; }
        public decimal? DownPayment { get; set; }
        public int? LoanTermMonths { get; set; }

        // ===== ✨ MỚI: Phiên bản & màu xe khách quan tâm =====
        // Có thể null lúc khách gửi (BE tự gán mặc định nếu null)
        public int? CarPricingVersionId { get; set; }
        public int? CarColorId { get; set; }

        // Log nhân viên append theo dòng thời gian
        public string? Note { get; set; }

        // Pending / Consulting / Success / Failed / Cancelled
        public string Status { get; set; } = null!;

        // Sales nào đang pickup
        public int? UserId { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public Car? Car { get; set; }
        public Showroom? Showroom { get; set; }
        public User? User { get; set; }
        public CarPricingVersion? CarPricingVersion { get; set; }
        public CarColor? CarColor { get; set; }
    }
}
