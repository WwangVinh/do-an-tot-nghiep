using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    // Dùng chung cho cả NotificationController và ChatHub (Tin nhắn hệ thống)
    public class NotificationDto
    {
        public int NotificationId { get; set; }
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public string? ActionUrl { get; set; }
        public string NotificationType { get; set; } = "System";
        public bool IsRead { get; set; }
        public string CreatedAt { get; set; } = null!;
    }
}
