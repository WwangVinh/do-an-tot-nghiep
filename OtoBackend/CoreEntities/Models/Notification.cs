using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreEntities.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        // 1. Gửi cho 1 cá nhân cụ thể (Nullable vì có thể gửi cho cả Showroom)
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        // 2. Gửi cho toàn bộ nhân viên trong 1 Showroom (Nullable vì có thể gửi cá nhân)
        public int? ShowroomId { get; set; }
        [ForeignKey("ShowroomId")]
        public virtual Showroom? Showroom { get; set; }

        // Tiêu đề thông báo (VD: "Yêu cầu tư vấn mới!")
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        // Nội dung chi tiết (VD: "Khách hàng Lê Thế Anh đang chờ ở phòng chat số 5.")
        [MaxLength(500)]
        public string? Content { get; set; }

        // CÁI ĂN TIỀN LÀ ĐÂY: Link để nhân viên click vào (VD: "/admin/chat/5")
        [MaxLength(255)]
        public string? ActionUrl { get; set; }

        public string? RoleTarget { get; set; }

        // Đã đọc hay chưa? (Dùng để tắt cái chấm đỏ trên cái chuông 🔔)
        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string NotificationType { get; set; } = "System";
    }
}