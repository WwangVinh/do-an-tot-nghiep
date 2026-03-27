using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = null!;

        // Thông tin User đính kèm để FE vẽ giao diện
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = null!; // Cột quan trọng nhất FE đang cần!

        // Cực kỳ quan trọng cho dự án của bạn: Trả về ID showroom để FE biết người này quản lý chi nhánh nào
        public int? ShowroomId { get; set; }
    }
}
