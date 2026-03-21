using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    public class ShowroomRequestDto
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? Hotline { get; set; }
    }

    public class ShowroomResponseDto
    {
        public int ShowroomId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? Hotline { get; set; }
    }
}
