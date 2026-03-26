using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    public class SendMessageRequestDto
    {
        public int SessionId { get; set; }
        public string SenderType { get; set; } = null!; // "Customer" hoặc "Agent"
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
