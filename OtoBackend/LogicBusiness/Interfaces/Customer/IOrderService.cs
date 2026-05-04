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
        Task<(bool Success, string Message, string? OrderCode, int? OrderId)> CreateGuestOrderAsync(CreateOrderDto dto);
        Task<OrderLookupDto?> LookupOrderAsync(string phone, string orderCode);
        Task<(bool Success, decimal DiscountPercentage, string Message)> CheckPromotionAsync(string code, int carId);
        Task<List<OrderLookupDto>> GetOrdersByPhoneAsync(string phone);
        Task<(bool Success, string Message)> CancelOrderAsync(int orderId);
        Task<List<ShowroomPickerDto>> GetAvailableShowroomsAsync();
        Task<List<ShowroomPickerDto>> GetShowroomsByCarIdAsync(int carId);
    }
}