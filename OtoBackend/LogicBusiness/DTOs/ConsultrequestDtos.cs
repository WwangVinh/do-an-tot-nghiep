using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    public class ConsultRequestCreateDto
    {
        public int CarId { get; set; }
        public int ShowroomId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string Phone { get; set; } = null!;

        // "Quotation" hoặc "Installment"
        public string RequestType { get; set; } = null!;

        public string? CustomerNote { get; set; }

        // ===== Chỉ áp dụng khi RequestType = "Installment" =====
        public decimal? MonthlyIncome { get; set; }
        public decimal? DownPayment { get; set; }
        public int? LoanTermMonths { get; set; }

        // ===== ✨ MỚI: Phiên bản & màu xe =====
        // Optional — nếu null BE tự gán mặc định (bản rẻ nhất + màu đầu tiên)
        public int? CarPricingVersionId { get; set; }
        public int? CarColorId { get; set; }
    }

    public class ConsultPickupDto
    {
        public string? Note { get; set; }
    }

    public class ConsultResultDto
    {
        public bool IsSuccess { get; set; }
        public string ResultNote { get; set; } = null!;
    }

    public class ConsultCancelByPhoneDto
    {
        public string Phone { get; set; } = null!;
        public string? Reason { get; set; }
    }

    public class ConsultQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public string? Status { get; set; }
        public string? RequestType { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
    }

    public static class ConsultRequestType
    {
        public const string Quotation = "Quotation";
        public const string Installment = "Installment";
    }

    public static class ConsultStatus
    {
        public const string Pending = "Pending";
        public const string Consulting = "Consulting";
        public const string Success = "Success";
        public const string Failed = "Failed";
        public const string Cancelled = "Cancelled";
    }
}