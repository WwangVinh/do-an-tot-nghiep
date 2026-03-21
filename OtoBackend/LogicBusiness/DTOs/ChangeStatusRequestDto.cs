using CoreEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    public class ChangeStatusRequestDto
    {
        public CarStatus NewStatus { get; set; }
    }
}
