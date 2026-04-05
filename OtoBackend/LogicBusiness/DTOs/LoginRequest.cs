using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntities.Models.DTOs
{
    // Đăng nhập
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }


    // Đăng nhập bên quản trị
    public class LoginResponseDto
    {
        public string Token { get; set; } = null!;
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = null!; // phân quyền cho FE biết người này là Admin, Manager hay Staff để hiển thị giao diện phù hợp
        public int? ShowroomId { get; set; } // Nếu là nhân viên có showroom thì trả về showroomId, còn Admin thì để null
    }

    // Đăng ký người dùng mới
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
    }

    // Tạo tài khoản nhân viên mới (Admin hoặc Manager mới có quyền tạo)
    public class StaffAccountRequestDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Phone { get; set; }

        // Quyền của tài khoản: "ShowroomManager" hoặc "ShowroomSales"
        [Required(ErrorMessage = "Bắt buộc phải truyền Quyền (Role) vào!")]
        [RegularExpression("^(ShowroomManager|ShowroomSales)$",
            ErrorMessage = "Hệ thống chỉ cho phép tạo quyền 'ShowroomManager' hoặc 'ShowroomSales' thôi!")]
        public string Role { get; set; } = null!;
        public int ShowroomId { get; set; }        // ID của Showroom mà người này sẽ quản lý
    }
}
