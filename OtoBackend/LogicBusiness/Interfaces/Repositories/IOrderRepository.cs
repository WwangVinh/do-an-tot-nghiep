using CoreEntities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task UpdateOrderAsync(Order order);

        Task<Order> CreateOrderAsync(Order order);
        Task<Order?> GetOrderByPhoneAndCodeAsync(string phone, string orderCode);

        Task<Promotion?> GetPromotionByCodeAsync(string code);
        Task UpdatePromotionAsync(Promotion promotion);
        Task<decimal> GetCarPriceAsync(int carId);
        Task<List<Accessory>> GetAccessoriesByIdsAsync(List<int> accessoryIds);

        //phần thanh toán
        Task AddPaymentTransactionAsync(PaymentTransaction payment);
        Task<decimal> GetTotalPaidAmountAsync(int orderId);
        Task<bool> CheckOrderForReviewAsync(string phone, string orderCode);
        // 1. Trong IOrderRepository.cs
        Task<bool> HasOrderedCarAsync(string phone, int carId);

    }
}

