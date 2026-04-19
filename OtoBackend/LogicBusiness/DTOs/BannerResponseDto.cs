using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    public class BannerResponseDto
    {
        public int BannerId { get; set; }
        public string BannerName { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public string? LinkUrl { get; set; }
        public int? Position { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    // Dùng khi Admin thêm mới hoặc cập nhật Banner
    public class BannerCreateUpdateDto
    {
        public string BannerName { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public string? LinkUrl { get; set; }
        public int? Position { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
