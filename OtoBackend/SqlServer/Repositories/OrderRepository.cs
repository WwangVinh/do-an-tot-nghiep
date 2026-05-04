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

        // ✅ Phân trang + lọc + tìm kiếm
        public async Task<(List<Order> Items, int Total)> GetOrdersPagedAsync(
            string? search, string? status, string? paymentStatus, int page, int pageSize)
        {
            var query = _context.Orders
                .Include(o => o.Car)
                .Include(o => o.Staff)
                .Include(o => o.Showroom)
                .AsQueryable();

            // Tìm kiếm theo mã đơn, tên khách, SĐT
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(o =>
                    (o.OrderCode != null && o.OrderCode.ToLower().Contains(s)) ||
                    o.FullName.ToLower().Contains(s) ||
                    o.Phone.Contains(s));
            }

            // Lọc theo trạng thái đơn
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(o => o.Status == status);

            // Lọc theo trạng thái thanh toán
            if (!string.IsNullOrWhiteSpace(paymentStatus))
                query = query.Where(o => o.PaymentStatus == paymentStatus);

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync()
        {
            return await _context.Orders
                .Include(o => o.Car)
                .Include(o => o.Staff)
                .Include(o => o.Showroom) // ✅
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task<Order?> GetOrderByIdWithDetailsAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Car)
                .Include(o => o.Staff)
                .Include(o => o.Showroom) // ✅
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Car)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Accessory)
                .Include(o => o.PaymentTransactions)
                .FirstOrDefaultAsync(o => o.OrderId == id);
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
        public async Task<string?> GetCustomerNameFromOrderAsync(string phone, int carId)
        {
            var order = await _context.Orders
                .Where(o => o.Phone == phone && o.CarId == carId) // Nếu bảng Order của ní lưu nhiều xe thì dùng .Any(i => i.CarId == carId) nhé
                .OrderByDescending(o => o.OrderDate)
                .FirstOrDefaultAsync();

            return order?.FullName;
        }
        public async Task<List<Order>> GetOrdersByPhoneAsync(string phone)
        {
            return await _context.Orders
                .Include(o => o.Car)
                .Include(o => o.OrderItems).ThenInclude(i => i.Accessory)
                .Where(o => o.Phone == phone)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
        public async Task<decimal> GetPricingVersionPriceAsync(int pricingVersionId)
        {
            var version = await _context.CarPricingVersions.FindAsync(pricingVersionId);
            return version?.PriceVnd ?? 0;
        }

        public async Task<Showroom?> GetShowroomByIdAsync(int showroomId)
        {
            return await _context.Showrooms.FindAsync(showroomId);
        }

        public async Task<List<Showroom>> GetAllShowroomsAsync()
        {
            return await _context.Showrooms
                .OrderBy(s => s.Name)
                .ToListAsync();
        }
        public async Task<List<Showroom>> GetShowroomsByCarIdAsync(int carId)
        {
            return await _context.CarInventories
                .Where(ci => ci.CarId == carId && ci.Quantity > 0)
                .Select(ci => ci.Showroom)
                .Where(s => s != null)
                .Distinct()
                .OrderBy(s => s.Name)
                .ToListAsync();
        }
    }
}