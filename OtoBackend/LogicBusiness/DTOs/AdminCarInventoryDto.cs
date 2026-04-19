using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    public class AdminCarInventoryDto
    {
        public int CarId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string DisplayStatus { get; set; } = null!; // OnDisplay, InWarehouse, TestDriveOnly
        public string? MainImageUrl { get; set; }
    }
}
