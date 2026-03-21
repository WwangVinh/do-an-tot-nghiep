using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    public class ShowroomCarResponseDto
    {
        public int CarId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string? MainImageUrl { get; set; }
        public int Quantity { get; set; }
        public string DisplayStatus { get; set; } = null!;
    }
}
