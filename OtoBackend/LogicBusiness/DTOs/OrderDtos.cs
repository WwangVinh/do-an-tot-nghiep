using System;
using System.Collections.Generic;

namespace CoreEntities.DTOs
{
    public class OrderAdminDto
    {
        public int OrderId { get; set; }
        public string? OrderCode { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? CarName { get; set; }
        public decimal FinalAmount { get; set; }
        public string? Status { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? StaffName { get; set; }
        public string? AdminNote { get; set; }
        public int? ShowroomId { get; set; }
        public string? ShowroomName { get; set; }
    }

    public class CreateOrderDto
    {
        public int? PricingVersionId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? CustomerNote { get; set; }
        public int? CarId { get; set; }
        public int? ShowroomId { get; set; }
        public string? PromotionCode { get; set; }
        public List<int> AccessoryIds { get; set; } = new List<int>();

        // ✅ Phí lăn bánh nếu khách nhờ showroom nộp hộ (trước bạ, biển số, đường bộ...)
        public decimal RollingFees { get; set; } = 0;
    }

    public class OrderLookupDto
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int? CarId { get; set; }
        public string CarName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public decimal FinalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public List<string> Accessories { get; set; } = new List<string>();
    }

    public class OrderDetailAdminDto
    {
        public int OrderId { get; set; }
        public string? OrderCode { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? CustomerNote { get; set; }
        public string? AdminNote { get; set; }
        public string? Status { get; set; }
        public string? PaymentStatus { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string? StaffName { get; set; }
        public string? CarName { get; set; }
        public int? ShowroomId { get; set; }
        public string? ShowroomName { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public List<PaymentTransactionDto> Payments { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int OrderItemId { get; set; }
        public string ItemType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class PaymentTransactionDto
    {
        public int TransactionId { get; set; }
        public decimal? Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; }
        public DateTime? TransactionDate { get; set; }
    }

    public class AddPaymentDto
    {
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = "Success";
    }

    // ─── Tham số tìm kiếm / lọc / phân trang cho trang admin ───
    public class OrderQueryParams
    {
        public string? Search { get; set; }          // Tìm theo mã đơn, tên, SĐT
        public string? Status { get; set; }          // Pending | Confirmed | Completed | Cancelled ...
        public string? PaymentStatus { get; set; }   // Unpaid | Deposited | Paid
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    // ─── Wrapper phân trang chung ───
    public class PagedResult<T>
    {
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public List<T> Data { get; set; } = new();
    }

    /// Dùng để hiển thị danh sách showroom cho khách chọn khi đặt xe
    public class ShowroomPickerDto
    {
        public int ShowroomId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
        public string? Hotline { get; set; }
    }
}