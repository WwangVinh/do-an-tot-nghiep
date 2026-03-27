using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    // 1. Dành cho khách hàng lúc nộp đơn
    public class ConsignmentCreateDto
    {
        public string Brand { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year { get; set; }
        public decimal Mileage { get; set; }
        public string? ConditionDescription { get; set; }
        public decimal ExpectedPrice { get; set; }
    }

    // 2. Dành cho sếp lúc duyệt/thẩm định xe
    public class ConsignmentUpdateDto
    {
        public string Status { get; set; } = null!; // Pending, Appraising, Approved, Rejected, Completed
        public decimal? AgreedPrice { get; set; }
        public decimal? CommissionRate { get; set; }
        public int? LinkedCarId { get; set; } // Khi xe được đăng bán thì nhét ID xe vào đây
    }

    // 3. Dữ liệu trả về cho cả 2 bên xem
    public class ConsignmentResponseDto
    {
        public int ConsignmentId { get; set; }
        public int? UserId { get; set; }
        public string CustomerName { get; set; } = null!;
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
    }
}
