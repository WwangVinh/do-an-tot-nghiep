using System.ComponentModel.DataAnnotations;

namespace LogicBusiness.DTOs
{
    public class UserUpdateRequestDto
    {
        [Required]
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Phone { get; set; }

        // Staff roles only
        [Required]
        [RegularExpression("^(ShowroomManager|ShowroomSales|SalesManager|Sales|Technician)$")]
        public string Role { get; set; } = null!;

        [Required]
        public int ShowroomId { get; set; }

        // Optional: allow changing status explicitly
        public string? Status { get; set; } // "Active" | "Inactive"
    }
}

