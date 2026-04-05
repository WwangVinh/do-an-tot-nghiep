using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    // DTO nhận dữ liệu từ React để tạo mới 1 phiên chat
    public class CreateSessionRequestDto
    {
        public string? GuestToken { get; set; }
        public int? AssignedTo { get; set; }
        public int? ShowroomId { get; set; }
    }

    // DTO trả dữ liệu phòng chat về cho React
    public class ChatSessionResponseDto
    {
        public int SessionId { get; set; }
        public int? UserId { get; set; }
        public string? GuestToken { get; set; }
        public string Status { get; set; } = null!;
    }

    // DTO để React hiển thị danh sách nhân viên có thể chat
    public class StaffForChatDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public int? ShowroomId { get; set; }
    }

    // DTO nhận dữ liệu từ React để gửi tin nhắn mới
    public class SendMessageRequestDto
    {
        public int SessionId { get; set; }
        public string SenderType { get; set; } = null!; 
        public string MessageText { get; set; } = null!;
    }

    // Dữ liệu Backend trả về để Frontend vẽ lên màn hình
    public class ChatMessageResponseDto
    {
        public int MessageId { get; set; }
        public int SessionId { get; set; }
        public string SenderType { get; set; } = null!;
        public string MessageText { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
