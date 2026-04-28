using System;
using System.Collections.Generic;

namespace CoreEntities.DTOs
{
    // ==========================================
    // 1. DTO DÀNH CHO TRANG QUẢN TRỊ (ADMIN)
    // ==========================================
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
    }

    // ==========================================
    // 2. DTO DÀNH CHO KHÁCH VÃNG LAI (GỬI ĐẶT CỌC)
    // ==========================================
    public class CreateOrderDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? CustomerNote { get; set; }
        public int? CarId { get; set; }

        public string? PromotionCode { get; set; }

        // Danh sách ID phụ kiện khách chọn thêm (nếu có)
        public List<int> AccessoryIds { get; set; } = new List<int>();
    }

    // ==========================================
    // 3. DTO DÀNH CHO KHÁCH VÃNG LAI (TRA CỨU ĐƠN)
    // ==========================================
    public class OrderLookupDto
    {
        public string OrderCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CarName { get; set; } = string.Empty;
        public decimal FinalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;

        // Danh sách tên phụ kiện để hiện lên giao diện cho khách xem
        public List<string> Accessories { get; set; } = new List<string>();
    }
}