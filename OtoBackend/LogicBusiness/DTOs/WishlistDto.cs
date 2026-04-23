using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Helpers
{

    // Dùng chung cho cả GET Wishlist của Customer và GET Wishlist của Admin (Xem tất cả khách hàng đã thích xe nào)
    public class WishlistResponseDto
    {
        public int WishlistId { get; set; }
        public int CarId { get; set; }
        public string CarName { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public int Year { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string Status { get; set; } = null!;
    }
}
