using CoreEntities.DTOs;
using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Interfaces.Shared;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LogicBusiness.Services.Admin
{
    public class OrderAdminService : IOrderAdminService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly INotificationService _notiService;

        public OrderAdminService(IOrderRepository orderRepo, INotificationService notiService)
        {
            _orderRepo = orderRepo;
            _notiService = notiService;
        }

        public async Task<PagedResult<OrderAdminDto>> GetAdminOrdersAsync(OrderQueryParams q)
        {
            var (items, total) = await _orderRepo.GetOrdersPagedAsync(
                q.Search, q.Status, q.PaymentStatus, q.Page, q.PageSize);

            var dtoList = items.Select(o => new OrderAdminDto
            {
                OrderId = o.OrderId,
                OrderCode = o.OrderCode,
                FullName = o.FullName,
                Phone = o.Phone,
                CarName = o.Car?.Name ?? "Chưa rõ",
                FinalAmount = o.FinalAmount,
                Status = o.Status,
                PaymentStatus = o.PaymentStatus,
                OrderDate = o.OrderDate,
                StaffName = o.Staff?.FullName ?? "Chưa phân công",
                AdminNote = o.AdminNote,
                ShowroomId = o.ShowroomId,
                ShowroomName = o.Showroom?.Name ?? "Chưa chọn showroom",
            }).ToList();

            return new PagedResult<OrderAdminDto>
            {
                TotalItems = total,
                CurrentPage = q.Page,
                PageSize = q.PageSize,
                Data = dtoList,
            };
        }

        /// Xem chi tiết đơn hàng kèm danh sách phụ kiện và lịch sử thanh toán
        public async Task<OrderDetailAdminDto?> GetOrderDetailAsync(int orderId)
        {
            var order = await _orderRepo.GetOrderByIdWithDetailsAsync(orderId);
            if (order == null) return null;

            return new OrderDetailAdminDto
            {
                OrderId = order.OrderId,
                OrderCode = order.OrderCode,
                FullName = order.FullName,
                Phone = order.Phone,
                Email = order.Email,
                CustomerNote = order.CustomerNote,
                AdminNote = order.AdminNote,
                Status = order.Status,
                PaymentStatus = order.PaymentStatus,
                Subtotal = order.Subtotal,
                DiscountAmount = order.DiscountAmount,
                FinalAmount = order.FinalAmount,
                OrderDate = order.OrderDate,
                LastUpdated = order.LastUpdated,
                StaffName = order.Staff?.FullName ?? "Chưa phân công",
                CarName = order.Car?.Name ?? "Chưa rõ",
                ShowroomId = order.ShowroomId,
                ShowroomName = order.Showroom?.Name ?? "Chưa chọn showroom",
                Items = order.OrderItems.Select(i => new OrderItemDto
                {
                    OrderItemId = i.OrderItemId,
                    ItemType = i.ItemType ?? "",
                    Name = i.ItemType == "Car" ? (i.Car?.Name ?? "Xe") : (i.Accessory?.Name ?? "Phụ kiện"),
                    Quantity = i.Quantity ?? 1,
                    Price = i.Price ?? 0,
                }).ToList(),
                Payments = order.PaymentTransactions?.Select(p => new PaymentTransactionDto
                {
                    TransactionId = p.TransactionId,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    Status = p.Status,
                    TransactionDate = p.TransactionDate,
                }).ToList() ?? new(),
            };
        }

        /// Nhân viên tạo đơn hộ khách trực tiếp tại showroom
        public async Task<(bool Success, string Message, string? OrderCode)> CreateOrderForCustomerAsync(CreateOrderDto dto, int staffId)
        {
            var order = new Order
            {
                FullName = dto.FullName,
                Phone = dto.Phone,
                Email = dto.Email,
                CustomerNote = dto.CustomerNote,
                CarId = dto.CarId,
                ShowroomId = dto.ShowroomId, // ✅ Lưu showroom staff đang làm việc
                OrderDate = DateTime.Now,
                Status = "Confirmed",
                PaymentStatus = "Unpaid",
                OrderCode = "OTO-" + Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper(),
                SecretToken = Guid.NewGuid().ToString(),
                Subtotal = 0,
                DiscountAmount = 0,
                FinalAmount = 0,
                StaffId = staffId,
            };

            // Tính tiền xe
            if (dto.CarId.HasValue)
            {
                decimal carPrice = await _orderRepo.GetCarPriceAsync(dto.CarId.Value);
                order.Subtotal += carPrice;
                order.OrderItems.Add(new OrderItem { CarId = dto.CarId, ItemType = "Car", Quantity = 1, Price = carPrice });
            }

            // Tính tiền phụ kiện
            if (dto.AccessoryIds != null && dto.AccessoryIds.Any())
            {
                var accessories = await _orderRepo.GetAccessoriesByIdsAsync(dto.AccessoryIds);
                foreach (var acc in accessories)
                {
                    order.Subtotal += acc.Price;
                    order.OrderItems.Add(new OrderItem { AccessoryId = acc.AccessoryId, ItemType = "Accessory", Quantity = 1, Price = acc.Price });
                }
            }

            // Áp mã giảm giá
            if (!string.IsNullOrWhiteSpace(dto.PromotionCode))
            {
                var promotion = await _orderRepo.GetPromotionByCodeAsync(dto.PromotionCode);
                if (promotion != null)
                {
                    if (promotion.MaxUsage.HasValue && promotion.CurrentUsage >= promotion.MaxUsage.Value)
                        return (false, "Mã giảm giá này đã hết lượt sử dụng!", null);

                    if (promotion.CarId.HasValue && promotion.CarId != dto.CarId)
                        return (false, "Mã giảm giá không áp dụng cho xe này!", null);

                    if (promotion.DiscountPercentage.HasValue)
                        order.DiscountAmount = (order.Subtotal * promotion.DiscountPercentage.Value) / 100;

                    order.PromotionId = promotion.PromotionId;
                    promotion.CurrentUsage += 1;
                    await _orderRepo.UpdatePromotionAsync(promotion);
                }
                else
                {
                    return (false, "Mã giảm giá không hợp lệ!", null);
                }
            }

            order.FinalAmount = Math.Max(0, order.Subtotal - order.DiscountAmount);
            await _orderRepo.CreateOrderAsync(order);

            // Thông báo cho Manager
            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: null,
                roleTarget: AppRoles.Manager,
                title: "Đơn hàng mới từ showroom! 🏪",
                content: $"Nhân viên vừa tạo đơn {order.OrderCode} cho khách {dto.FullName} ({dto.Phone}). Tổng tiền: {order.FinalAmount:N0}đ.",
                actionUrl: $"/admin/orders/detail/{order.OrderId}",
                type: "Order"
            );

            return (true, "Tạo đơn hàng thành công!", order.OrderCode);
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status, string adminNote)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId);
            if (order == null) return false;

            order.Status = status;
            order.AdminNote = adminNote;
            order.LastUpdated = DateTime.Now;

            await _orderRepo.UpdateOrderAsync(order);

            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: null,
                roleTarget: AppRoles.Manager,
                title: "Cập nhật trạng thái đơn hàng 🔄",
                content: $"Đơn hàng {order.OrderCode} vừa được chuyển sang trạng thái: {status}.",
                actionUrl: $"/admin/orders/detail/{orderId}",
                type: "OrderStatus"
            );

            return true;
        }

        public async Task<(bool Success, string Message)> AddPaymentAsync(int orderId, LogicBusiness.DTOs.AddPaymentDto dto)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId);
            if (order == null) return (false, "Không tìm thấy đơn hàng!");

            var payment = new PaymentTransaction
            {
                OrderId = orderId,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                Status = dto.Status,
                TransactionDate = DateTime.Now
            };

            await _orderRepo.AddPaymentTransactionAsync(payment);

            var totalPaid = await _orderRepo.GetTotalPaidAmountAsync(orderId);

            if (totalPaid == 0)
                order.PaymentStatus = "Unpaid";
            else if (totalPaid < order.FinalAmount)
                order.PaymentStatus = "Deposited";
            else
                order.PaymentStatus = "Paid";

            order.LastUpdated = DateTime.Now;
            await _orderRepo.UpdateOrderAsync(order);

            string statusVN = order.PaymentStatus == "Paid" ? "Đã thanh toán đủ" : "Đã cọc";
            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: null,
                roleTarget: $"{AppRoles.Manager},{AppRoles.Admin}",
                title: "Tiền về tài khoản! 💰",
                content: $"Đơn hàng {order.OrderCode} vừa được ghi nhận thanh toán {dto.Amount:N0}đ qua {dto.PaymentMethod}. Trạng thái: {statusVN}.",
                actionUrl: $"/admin/orders/detail/{orderId}",
                type: "Payment"
            );

            return (true, "Thêm lịch sử thanh toán thành công!");
        }
    }
}