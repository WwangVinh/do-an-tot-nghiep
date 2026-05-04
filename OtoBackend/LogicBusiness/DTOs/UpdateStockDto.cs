using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [RegularExpression("^(Available|OnDisplay|Out of stock)$",
            ErrorMessage = "Trạng thái sai rồi! Chỉ được nhập: Available, OnDisplay, hoặc Out of stock")]
        public string DisplayStatus { get; set; } = "Available";

        public string? Color { get; set; }
    }
}
