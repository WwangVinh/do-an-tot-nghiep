using CoreEntities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        // ✅ Có filter + phân trang — dùng cho trang admin
        Task<(List<Order> Items, int Total)> GetOrdersPagedAsync(
            string? search, string? status, string? paymentStatus, int page, int pageSize);

        Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        // Load đầy đủ OrderItems + Accessory + PaymentTransactions cho trang chi tiết
        Task<Order?> GetOrderByIdWithDetailsAsync(int id);
        Task UpdateOrderAsync(Order order);
        Task<Order> CreateOrderAsync(Order order);
        Task<Order?> GetOrderByPhoneAndCodeAsync(string phone, string orderCode);
        Task<Promotion?> GetPromotionByCodeAsync(string code);
        Task UpdatePromotionAsync(Promotion promotion);
        Task<decimal> GetCarPriceAsync(int carId);
        Task<string?> GetCustomerNameFromOrderAsync(string phone, int carId);
        Task<List<Accessory>> GetAccessoriesByIdsAsync(List<int> accessoryIds);
        Task AddPaymentTransactionAsync(PaymentTransaction payment);
        Task<decimal> GetTotalPaidAmountAsync(int orderId);
        Task<bool> CheckOrderForReviewAsync(string phone, string orderCode);
        Task<bool> HasOrderedCarAsync(string phone, int carId);
        Task<List<Order>> GetOrdersByPhoneAsync(string phone);
        Task<decimal> GetPricingVersionPriceAsync(int pricingVersionId);
        Task<Showroom?> GetShowroomByIdAsync(int showroomId);
        Task<List<Showroom>> GetAllShowroomsAsync();
        Task<List<Showroom>> GetShowroomsByCarIdAsync(int carId);
    }
}