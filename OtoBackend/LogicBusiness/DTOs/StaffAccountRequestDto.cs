using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    public class StaffAccountRequestDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!; // Admin sẽ đặt pass mặc định cho nhân viên
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Phone { get; set; }

        // Quyền của tài khoản: "ShowroomManager" hoặc "ShowroomSales"
        public string Role { get; set; } = null!;

        // ID của Showroom mà người này sẽ quản lý
        public int ShowroomId { get; set; }
    }
}
