using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    public class CarSpecificationDto
    {
        public string Category { get; set; } = null!;  // Ví dụ: Động cơ
        public string SpecName { get; set; } = null!;  // Ví dụ: Loại động cơ
        public string SpecValue { get; set; } = null!; // Ví dụ: 1.5L Turbo
    }
}
