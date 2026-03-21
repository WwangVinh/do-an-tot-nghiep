using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    public class BookingCreateDto
    {
        public int CarId { get; set; }
        public int ShowroomId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateOnly BookingDate { get; set; }
        public string? BookingTime { get; set; }

        public string? Note { get; set; }
        public int? UserId { get; set; }
    }
}