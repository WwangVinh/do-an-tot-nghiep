using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntities.Models
{
    public class OrderAdminViewModel
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string CarName { get; set; }
        public decimal FinalAmount { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }

        // Thêm các trường này để Admin nhìn phát hiểu luôn
        public string PaymentStatus { get; set; } // Trạng thái tiền nong
        public string StaffName { get; set; }     // Tên nhân viên phụ trách
        public string AdminNote { get; set; }    // Ghi chú mới nhất của Sale
    }
}
