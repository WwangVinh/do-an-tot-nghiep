using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    public class UpdateStockDto
    {
        public int CarId { get; set; }
        public int ShowroomId { get; set; }
        public int Quantity { get; set; }
    }
}
