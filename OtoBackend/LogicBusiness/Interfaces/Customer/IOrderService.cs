using CoreEntities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Customer
{
    public interface IOrderService
    {
        Task<(bool Success, string Message, string? OrderCode)> CreateGuestOrderAsync(CreateOrderDto dto);
        Task<OrderLookupDto?> LookupOrderAsync(string phone, string orderCode);
    }
}
