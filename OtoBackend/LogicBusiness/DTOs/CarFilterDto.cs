using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    public class CarFilterDto
    {
        public string? Keyword { get; set; }     // Tìm theo Tên xe hoặc Hãng )
        public string? Brand { get; set; }       // Lọc chính xác theo Hãng
        public decimal? MinPrice { get; set; }   // Giá từ...
        public decimal? MaxPrice { get; set; }   // Giá đến...
        public int? Condition { get; set; }      // 0: Xe Mới, 1: Xe Cũ (Dựa theo Enum của ní)
        public int? Status { get; set; }         // Chỉ dành cho Admin (Lọc xe Nháp, Ẩn, Hiển thị)
    }
}