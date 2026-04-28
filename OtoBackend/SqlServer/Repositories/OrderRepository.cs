using CoreEntities.Models;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlServer.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OtoContext _context;

        public OrderRepository(OtoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync()
        {
            // Include để lấy luôn thông tin Xe và Nhân viên (tránh lỗi N+1 query)
            return await _context.Orders
                .Include(o => o.Car)
                .Include(o => o.Staff)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> GetOrderByPhoneAndCodeAsync(string phone, string orderCode)
        {
            // Lấy chi tiết đơn kèm Xe và Phụ kiện
            return await _context.Orders
                .Include(o => o.Car)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Accessory)
                .FirstOrDefaultAsync(o => o.Phone == phone && o.OrderCode == orderCode);
        }

        // 👉 THÊM VÀO CUỐI CLASS OrderRepository:

        public async Task<Promotion?> GetPromotionByCodeAsync(string code)
        {
            return await _context.Promotions.FirstOrDefaultAsync(p => p.Code == code);
        }

        public async Task UpdatePromotionAsync(Promotion promotion)
        {
            _context.Promotions.Update(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetCarPriceAsync(int carId)
        {
            var car = await _context.Cars.FindAsync(carId);
            return car?.Price ?? 0;
        }

        public async Task<List<Accessory>> GetAccessoriesByIdsAsync(List<int> accessoryIds)
        {
            return await _context.Accessories
                .Where(a => accessoryIds.Contains(a.AccessoryId))
                .ToListAsync();
        }


        //thanh toán
        public async Task AddPaymentTransactionAsync(PaymentTransaction payment)
        {
            await _context.PaymentTransactions.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetTotalPaidAmountAsync(int orderId)
        {
            // Tính tổng tất cả các lần nộp tiền thành công
            // Thêm ?? 0 để lỡ không có giao dịch nào thì trả về 0đ thay vì báo lỗi
            return await _context.PaymentTransactions
                .Where(p => p.OrderId == orderId && p.Status == "Success")
                .SumAsync(p => p.Amount ?? 0);
        }

        public async Task<bool> CheckOrderForReviewAsync(string phone, string orderCode)
        {
            // Chỉ những đơn hàng đã cọc hoặc thanh toán mới được review
            return await _context.Orders.AnyAsync(o =>
                o.Phone == phone &&
                o.OrderCode == orderCode &&
                (o.PaymentStatus == "Paid" || o.PaymentStatus == "Deposited"));
        }

        public async Task<bool> HasOrderedCarAsync(string phone, int carId)
        {
            return await _context.Orders.AnyAsync(o => o.Phone == phone && o.CarId == carId
                && (o.PaymentStatus == "Paid" || o.PaymentStatus == "Deposited"));
        }
    }
}

