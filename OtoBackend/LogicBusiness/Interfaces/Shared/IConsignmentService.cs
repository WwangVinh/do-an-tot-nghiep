using LogicBusiness.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Shared
{
    public interface IConsignmentService
    {
        Task<(bool Success, string Message)> CreateConsignmentAsync(int userId, string customerName, ConsignmentCreateDto dto);
        Task<(bool Success, string Message)> UpdateConsignmentStatusAsync(int consignmentId, ConsignmentUpdateDto dto, string adminRole);

    }
}
