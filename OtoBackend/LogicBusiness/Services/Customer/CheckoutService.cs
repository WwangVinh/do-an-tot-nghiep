using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Interfaces.Customer;
using PayOS;
using PayOS.Models;
using PayOS.Models.V2.PaymentRequests;
using System;
using System.Threading.Tasks;

namespace LogicBusiness.Services.Customer
{
    public class CheckoutService : ICheckoutService
    {
        private readonly PayOSClient _payOSClient;
        private readonly ISystemSettingAdminService _settingService;
        private readonly IOrderRepository _orderRepo;

        public CheckoutService(PayOSClient payOSClient, ISystemSettingAdminService settingService, IOrderRepository orderRepo)
        {
            _payOSClient = payOSClient;
            _settingService = settingService;
            _orderRepo = orderRepo;
        }

        public async Task<string> CreatePaymentLinkAsync(int orderId)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId);
            if (order == null) throw new Exception("Đơn hàng không tồn tại");

            // Lấy % đặt cọc từ SystemSettings
            var settingValue = await _settingService.GetSettingValueAsync("DepositPercentage");
            decimal percentage = decimal.TryParse(settingValue, out var p) ? p : 10;

            // Tính số tiền đặt cọc
            // ✅ Dùng Math.Round để tránh truncate — đây là fix chính
            int amountToPay = (int)Math.Round(order.FinalAmount * (percentage / 100m));

            // ✅ Validate: PayOS yêu cầu tối thiểu 1000đ
            if (amountToPay < 1000)
                throw new Exception($"Số tiền đặt cọc ({amountToPay:N0}đ) quá thấp. Vui lòng liên hệ showroom để được hỗ trợ.");

            // Description tối đa 25 ký tự theo quy định PayOS
            string description = $"Coc don {order.OrderCode}";
            if (description.Length > 25)
                description = description.Substring(0, 25);

            // ✅ Lấy FE URL từ SystemSettings hoặc fallback
            var feUrl = await _settingService.GetSettingValueAsync("FrontendUrl") ?? "http://localhost:5173";
            feUrl = feUrl.TrimEnd('/');

            string returnUrl = $"{feUrl}/payment-success";
            string cancelUrl = $"{feUrl}/payment-failed";

            var paymentRequest = new CreatePaymentLinkRequest
            {
                OrderCode = order.OrderId,
                Amount = amountToPay,
                Description = description,
                CancelUrl = cancelUrl,
                ReturnUrl = returnUrl
            };

            var paymentLink = await _payOSClient.PaymentRequests.CreateAsync(paymentRequest);
            return paymentLink.CheckoutUrl;
        }
    }
}