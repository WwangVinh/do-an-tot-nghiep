using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntities.Models.DTOs
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = null!;
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public int? ShowroomId { get; set; }
    }

    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
    }

    public class StaffAccountRequestDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Bắt buộc phải truyền Quyền (Role)!")]
        [RegularExpression("^(Manager|Sales|ShowroomSales|Technician|Marketing|Content)$",
            ErrorMessage = "Role không hợp lệ. Chỉ nhận: Manager, Sales, ShowroomSales, Technician, Marketing, Content.")]
        public string Role { get; set; } = null!;

        public int ShowroomId { get; set; }
    }
}