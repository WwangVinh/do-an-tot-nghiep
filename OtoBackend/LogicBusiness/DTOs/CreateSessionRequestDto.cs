using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    // DTO hứng dữ liệu từ React gửi lên khi tạo phòng
    public class CreateSessionRequestDto
    {
        public string? GuestToken { get; set; }
        public int? AssignedTo { get; set; }
    }

    // DTO trả dữ liệu phòng chat về cho React
    public class ChatSessionResponseDto
    {
        public int SessionId { get; set; }
        public int? UserId { get; set; }
        public string? GuestToken { get; set; }
        public string Status { get; set; } = null!;
    }

    public class StaffForChatDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public int? ShowroomId { get; set; } // Để sau này lọc nhân viên theo từng Showroom
    }
}
