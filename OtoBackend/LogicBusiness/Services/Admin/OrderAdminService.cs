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

        public async Task<IEnumerable<OrderAdminDto>> GetAdminOrdersAsync()
        {
            var orders = await _orderRepo.GetAllOrdersWithDetailsAsync();

            // Chuyển từ Entity (Order) sang DTO (OrderAdminDto)
            var dtoList = orders.Select(o => new OrderAdminDto
            {
                OrderId = o.OrderId,
                OrderCode = o.OrderCode,
                FullName = o.FullName,
                Phone = o.Phone,
                CarName = o.Car?.Name ?? "Chưa rõ", // Xử lý null an toàn
                FinalAmount = o.FinalAmount,
                Status = o.Status,
                PaymentStatus = o.PaymentStatus,
                OrderDate = o.OrderDate,
                StaffName = o.Staff?.FullName ?? "Chưa phân công",
                AdminNote = o.AdminNote
            }).ToList();

            return dtoList;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status, string adminNote)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId);
            if (order == null) return false;

            order.Status = status;
            order.AdminNote = adminNote;
            order.LastUpdated = DateTime.Now;

            await _orderRepo.UpdateOrderAsync(order);

            // 👇 THÔNG BÁO: Đổi trạng thái đơn
            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: null,
                roleTarget: AppRoles.Manager, // Chỉ cần báo Sếp để nắm tình hình
                title: "Cập nhật trạng thái đơn hàng 🔄",
                content: $"Đơn hàng {order.OrderCode} vừa được chuyển sang trạng thái: {status}.",
                actionUrl: $"/admin/orders/detail/{orderId}",
                type: "OrderStatus"
            );

            return true;
        }

        public async Task<(bool Success, string Message)> AddPaymentAsync(int orderId, AddPaymentDto dto)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId);
            if (order == null) return (false, "Không tìm thấy đơn hàng!");

            // 1. Tạo lịch sử giao dịch mới
            var payment = new PaymentTransaction
            {
                OrderId = orderId,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                Status = dto.Status,
                TransactionDate = DateTime.Now
            };

            await _orderRepo.AddPaymentTransactionAsync(payment);

            // 2. Lấy TỔNG SỐ TIỀN khách đã nộp (Bao gồm cả lần vừa nộp xong)
            var totalPaid = await _orderRepo.GetTotalPaidAmountAsync(orderId);

            // 3. Logic "Ma thuật": Tự động đổi PaymentStatus của đơn hàng
            if (totalPaid == 0)
            {
                order.PaymentStatus = "Unpaid";
            }
            else if (totalPaid < order.FinalAmount)
            {
                order.PaymentStatus = "Deposited"; // Đã cọc/Trả góp
            }
            else
            {
                order.PaymentStatus = "Paid"; // Đã thanh toán đủ
            }

            order.LastUpdated = DateTime.Now;

            // 4. Cập nhật Đơn hàng
            await _orderRepo.UpdateOrderAsync(order);

            string statusVN = order.PaymentStatus == "Paid" ? "Đã thanh toán đủ" : "Đã cọc";
            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: null,
                roleTarget: $"{AppRoles.Manager},{AppRoles.Admin}", // Tiền bạc thì Admin và Sếp bự phải biết
                title: "Tiền về tài khoản! 💰",
                content: $"Đơn hàng {order.OrderCode} vừa được ghi nhận thanh toán {dto.Amount:N0}đ qua {dto.PaymentMethod}. Trạng thái: {statusVN}.",
                actionUrl: $"/admin/orders/detail/{orderId}",
                type: "Payment"
            );

            return (true, "Thêm lịch sử thanh toán thành công!");
        }

    }
}

