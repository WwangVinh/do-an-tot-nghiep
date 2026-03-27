using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntities.Models.DTOs
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }

        public int? UserId { get; set; }
        public string? Username { get; set; }
        public string? Role { get; set; }
        public int? ShowroomId { get; set; }
    }
}
