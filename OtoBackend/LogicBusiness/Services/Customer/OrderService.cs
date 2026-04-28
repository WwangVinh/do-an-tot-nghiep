using CoreEntities.DTOs;
using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Customer;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Interfaces.Shared;
using Microsoft.EntityFrameworkCore;
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

        public async Task<(bool Success, string Message, string? OrderCode)> CreateGuestOrderAsync(CreateOrderDto dto)
        {
            // 1. Khởi tạo đơn hàng cơ bản
            var order = new Order
            {
                FullName = dto.FullName,
                Phone = dto.Phone,
                Email = dto.Email,
                CustomerNote = dto.CustomerNote,
                CarId = dto.CarId,
                OrderDate = DateTime.Now,
                Status = "Pending",
                PaymentStatus = "Unpaid",
                OrderCode = "OTO-" + Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper(),
                SecretToken = Guid.NewGuid().ToString(),
                Subtotal = 0,
                DiscountAmount = 0,
                FinalAmount = 0
            };

            // 2. Tính tiền Xe
            if (dto.CarId.HasValue)
            {
                decimal carPrice = await _orderRepo.GetCarPriceAsync(dto.CarId.Value);
                order.Subtotal += carPrice;
                order.OrderItems.Add(new OrderItem { CarId = dto.CarId, ItemType = "Car", Quantity = 1, Price = carPrice });
            }

            // 3. Tính tiền Phụ kiện
            if (dto.AccessoryIds != null && dto.AccessoryIds.Any())
            {
                var accessories = await _orderRepo.GetAccessoriesByIdsAsync(dto.AccessoryIds);

                foreach (var acc in accessories)
                {
                    order.Subtotal += acc.Price;
                    order.OrderItems.Add(new OrderItem { AccessoryId = acc.AccessoryId, ItemType = "Accessory", Quantity = 1, Price = acc.Price });
                }
            }

            // 4. XỬ LÝ MÃ GIẢM GIÁ
            if (!string.IsNullOrWhiteSpace(dto.PromotionCode))
            {
                var promotion = await _orderRepo.GetPromotionByCodeAsync(dto.PromotionCode);

                if (promotion != null)
                {
                    // A. Kiểm tra lượt dùng
                    if (promotion.MaxUsage.HasValue && promotion.CurrentUsage >= promotion.MaxUsage.Value)
                    {
                        return (false, "Mã giảm giá này đã hết lượt sử dụng!", null);
                    }

                    // B. Kiểm tra xem mã có giới hạn cho dòng xe cụ thể không (Nếu ní đã thêm cột CarId vào Promotions)
                    if (promotion.CarId.HasValue && promotion.CarId != dto.CarId)
                    {
                        return (false, "Mã giảm giá này không áp dụng cho dòng xe bạn chọn!", null);
                    }

                    // C. Tính số tiền giảm (Ví dụ: Giảm theo % trên tổng đơn)
                    if (promotion.DiscountPercentage.HasValue)
                    {
                        order.DiscountAmount = (order.Subtotal * promotion.DiscountPercentage.Value) / 100;
                    }

                    order.PromotionId = promotion.PromotionId;
                    promotion.CurrentUsage += 1;
                    await _orderRepo.UpdatePromotionAsync(promotion);
                }
                else
                {
                    return (false, "Mã giảm giá không hợp lệ!", null);
                }
            }

            // 5. Chốt tổng tiền cuối cùng
            order.FinalAmount = order.Subtotal - order.DiscountAmount;
            if (order.FinalAmount < 0) order.FinalAmount = 0; // Tránh tiền âm

            // 6. Lưu vào Database
            await _orderRepo.CreateOrderAsync(order);

            // 👇 7. BẮN THÔNG BÁO CÓ ĐƠN MỚI
            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: null, // Đơn online chung, nếu có ShowroomId trong DTO thì ní truyền vào nhé
                roleTarget: $"{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales}",
                title: "Có đơn đặt xe mới! 🛒",
                content: $"Khách hàng {dto.FullName} ({dto.Phone}) vừa đặt đơn hàng {order.OrderCode}. Tổng tiền: {order.FinalAmount:N0}đ. Anh em gọi chốt ngay!",
                actionUrl: $"/admin/orders/detail/{order.OrderId}",
                type: "Order"
            );

            return (true, "Đặt xe thành công!", order.OrderCode);
        }

        public async Task<OrderLookupDto?> LookupOrderAsync(string phone, string orderCode)
        {
            var order = await _orderRepo.GetOrderByPhoneAndCodeAsync(phone, orderCode);
            if (order == null) return null;

            return new OrderLookupDto
            {
                OrderCode = order.OrderCode,
                Status = order.Status,
                CarName = order.Car?.Name ?? "Chưa xác định",
                FinalAmount = order.FinalAmount,
                OrderDate = order.OrderDate ?? DateTime.Now,
                PaymentStatus = order.PaymentStatus,
                Accessories = order.OrderItems
                    .Where(x => x.ItemType == "Accessory" && x.Accessory != null)
                    .Select(x => x.Accessory.Name).ToList()
            };
        }
    }
}