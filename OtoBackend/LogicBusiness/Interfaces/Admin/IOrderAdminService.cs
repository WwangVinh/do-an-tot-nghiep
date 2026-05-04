using CoreEntities.DTOs;
using LogicBusiness.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Admin
{
    public interface IOrderAdminService
    {
        // ✅ Có phân trang + filter + search
        Task<PagedResult<OrderAdminDto>> GetAdminOrdersAsync(OrderQueryParams queryParams);
        Task<OrderDetailAdminDto?> GetOrderDetailAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string status, string adminNote);
        Task<(bool Success, string Message)> AddPaymentAsync(int orderId, LogicBusiness.DTOs.AddPaymentDto dto);
        Task<(bool Success, string Message, string? OrderCode)> CreateOrderForCustomerAsync(CreateOrderDto dto, int staffId);
    }
}