using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    public class ReviewSubmitDto
    {
        public int CarId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty; // SĐT dùng để "check var" với đơn hàng

        public string? OrderCode { get; set; } // Mã đơn hàng khách nhập vào

        public int Rating { get; set; } // Số sao (1-5)

        public string? Comment { get; set; } // Nội dung đánh giá
    }
}
