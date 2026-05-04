using CoreEntities.DTOs;
using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Customer;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Interfaces.Shared;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LogicBusiness.Services.Customer
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly INotificationService _notiService;

        public OrderService(IOrderRepository orderRepo, INotificationService notiService)
        {
            _orderRepo = orderRepo;
            _notiService = notiService;
        }

        public async Task<(bool Success, string Message, string? OrderCode, int? OrderId)> CreateGuestOrderAsync(CreateOrderDto dto)
        {
            var order = new Order
            {
                FullName = dto.FullName,
                Phone = dto.Phone,
                Email = dto.Email,
                CustomerNote = dto.CustomerNote,
                CarId = dto.CarId,
                ShowroomId = dto.ShowroomId, // ✅ Lưu showroom khách chọn
                OrderDate = DateTime.Now,
                Status = "Pending",
                PaymentStatus = "Unpaid",
                OrderCode = "OTO-" + Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper(),
                SecretToken = Guid.NewGuid().ToString(),
                Subtotal = 0,
                DiscountAmount = 0,
                FinalAmount = 0
            };

            if (dto.CarId.HasValue)
            {
                decimal carPrice;

                // ✅ Lấy giá theo phiên bản khách chọn thay vì giá xe chung
                if (dto.PricingVersionId.HasValue)
                {
                    carPrice = await _orderRepo.GetPricingVersionPriceAsync(dto.PricingVersionId.Value);
                    if (carPrice <= 0)
                        carPrice = await _orderRepo.GetCarPriceAsync(dto.CarId.Value);
                }
                else
                {
                    carPrice = await _orderRepo.GetCarPriceAsync(dto.CarId.Value);
                }

                order.Subtotal += carPrice;
                order.OrderItems.Add(new OrderItem { CarId = dto.CarId, ItemType = "Car", Quantity = 1, Price = carPrice });
            }

            if (dto.AccessoryIds != null && dto.AccessoryIds.Any())
            {
                var accessories = await _orderRepo.GetAccessoriesByIdsAsync(dto.AccessoryIds);
                foreach (var acc in accessories)
                {
                    order.Subtotal += acc.Price;
                    order.OrderItems.Add(new OrderItem { AccessoryId = acc.AccessoryId, ItemType = "Accessory", Quantity = 1, Price = acc.Price });
                }
            }

            if (!string.IsNullOrWhiteSpace(dto.PromotionCode))
            {
                var promotion = await _orderRepo.GetPromotionByCodeAsync(dto.PromotionCode);
                if (promotion != null)
                {
                    if (promotion.MaxUsage.HasValue && promotion.CurrentUsage >= promotion.MaxUsage.Value)
                        return (false, "Mã giảm giá này đã hết lượt sử dụng!", null, null);
                    if (promotion.CarId.HasValue && promotion.CarId != dto.CarId)
                        return (false, "Mã giảm giá này không áp dụng cho dòng xe bạn chọn!", null, null);
                    if (promotion.DiscountPercentage.HasValue)
                        order.DiscountAmount = (order.Subtotal * promotion.DiscountPercentage.Value) / 100;
                    order.PromotionId = promotion.PromotionId;
                    promotion.CurrentUsage += 1;
                    await _orderRepo.UpdatePromotionAsync(promotion);
                }
                else
                {
                    return (false, "Mã giảm giá không hợp lệ!", null, null);
                }
            }

            order.FinalAmount = order.Subtotal - order.DiscountAmount;
            if (order.FinalAmount < 0) order.FinalAmount = 0;

            // ✅ Cộng thêm phí lăn bánh nếu khách nhờ showroom nộp hộ
            if (dto.RollingFees > 0)
                order.FinalAmount += dto.RollingFees;

            await _orderRepo.CreateOrderAsync(order);

            await _notiService.CreateNotificationAsync(
                userId: null, showroomId: null,
                roleTarget: $"{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales}",
                title: "Có đơn đặt xe mới! 🛒",
                content: $"Khách hàng {dto.FullName} ({dto.Phone}) vừa đặt đơn hàng {order.OrderCode}. Tổng tiền: {order.FinalAmount:N0}đ.",
                actionUrl: $"/admin/orders/detail/{order.OrderId}",
                type: "Order"
            );

            return (true, "Đặt xe thành công!", order.OrderCode, order.OrderId);
        }

        public async Task<OrderLookupDto?> LookupOrderAsync(string phone, string orderCode)
        {
            var order = await _orderRepo.GetOrderByPhoneAndCodeAsync(phone, orderCode);
            if (order == null) return null;
            return new OrderLookupDto
            {
                OrderId = order.OrderId,
                OrderCode = order.OrderCode,
                Status = order.Status,
                CarId = order.CarId,
                CarName = order.Car?.Name ?? "Chưa xác định",
                FullName = order.FullName,
                Phone = order.Phone,
                FinalAmount = order.FinalAmount,
                OrderDate = order.OrderDate ?? DateTime.Now,
                PaymentStatus = order.PaymentStatus,
                Accessories = order.OrderItems.Where(x => x.ItemType == "Accessory" && x.Accessory != null).Select(x => x.Accessory.Name).ToList()
            };
        }

        public async Task<(bool Success, decimal DiscountPercentage, string Message)> CheckPromotionAsync(string code, int carId)
        {
            var promotion = await _orderRepo.GetPromotionByCodeAsync(code);
            if (promotion == null) return (false, 0, "Mã giảm giá không tồn tại!");
            if (promotion.MaxUsage.HasValue && promotion.CurrentUsage >= promotion.MaxUsage.Value)
                return (false, 0, "Mã giảm giá đã hết lượt sử dụng!");
            if (promotion.CarId.HasValue && promotion.CarId != carId)
                return (false, 0, "Mã này không áp dụng cho dòng xe này!");
            return (true, promotion.DiscountPercentage ?? 0, "Áp dụng mã thành công!");
        }

        public async Task<List<OrderLookupDto>> GetOrdersByPhoneAsync(string phone)
        {
            var orders = await _orderRepo.GetOrdersByPhoneAsync(phone);
            return orders.Select(o => new OrderLookupDto
            {
                OrderId = o.OrderId,
                OrderCode = o.OrderCode,
                Status = o.Status,
                CarId = o.CarId,
                CarName = o.Car?.Name ?? "Chưa xác định",
                FullName = o.FullName,
                Phone = o.Phone,
                FinalAmount = o.FinalAmount,
                OrderDate = o.OrderDate ?? DateTime.Now,
                PaymentStatus = o.PaymentStatus,
                Accessories = o.OrderItems.Where(x => x.ItemType == "Accessory" && x.Accessory != null).Select(x => x.Accessory.Name).ToList()
            }).ToList();
        }

        public async Task<(bool Success, string Message)> CancelOrderAsync(int orderId)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId);
            if (order == null) return (false, "Đơn hàng không tồn tại!");
            if (order.Status == "Cancelled") return (false, "Đơn hàng đã bị hủy!");
            order.Status = "Cancelled";
            await _orderRepo.UpdateOrderAsync(order);
            return (true, "Hủy đơn hàng thành công!");
        }

        public async Task<List<ShowroomPickerDto>> GetAvailableShowroomsAsync()
        {
            var showrooms = await _orderRepo.GetAllShowroomsAsync();
            return showrooms.Select(s => new ShowroomPickerDto
            {
                ShowroomId = s.ShowroomId,
                Name = s.Name ?? "",
                Province = s.Province,
                District = s.District,
                FullAddress = s.FullAddress,
                Hotline = s.Hotline,
            }).ToList();
        }
        
        public async Task<List<ShowroomPickerDto>> GetShowroomsByCarIdAsync(int carId)
        {
            var showrooms = await _orderRepo.GetShowroomsByCarIdAsync(carId);
            return showrooms.Select(s => new ShowroomPickerDto
            {
                ShowroomId = s.ShowroomId,
                Name = s.Name ?? "",
                Province = s.Province,
                District = s.District,
                FullAddress = s.FullAddress,
                Hotline = s.Hotline,
            }).ToList();
        }
    }
}