using CoreEntities.DTOs;
using LogicBusiness.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Admin
{
    public interface IOrderAdminService
    {
        Task<IEnumerable<OrderAdminDto>> GetAdminOrdersAsync();
        Task<bool> UpdateOrderStatusAsync(int orderId, string status, string adminNote);

        Task<(bool Success, string Message)> AddPaymentAsync(int orderId, AddPaymentDto dto);
    }
}

