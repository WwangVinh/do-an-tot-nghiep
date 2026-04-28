using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    public class AddPaymentDto
    {
        public decimal Amount { get; set; }

        // Nhập: Tiền mặt, Chuyển khoản, Quẹt thẻ...
        public string PaymentMethod { get; set; } = string.Empty;

        // Mặc định nộp tận tay thì là Success luôn
        public string Status { get; set; } = "Success";
    }
}
