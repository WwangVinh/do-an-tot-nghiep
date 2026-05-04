using Microsoft.AspNetCore.Mvc;
using LogicBusiness.Interfaces.Customer;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Repositories;
using System.Threading.Tasks;
using PayOS;

namespace OtoBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;
        private readonly IOrderAdminService _orderAdminService;
        private readonly IOrderRepository _orderRepo;
        private readonly PayOSClient _payOSClient;

        public CheckoutController(ICheckoutService checkoutService, IOrderAdminService orderAdminService, IOrderRepository orderRepo, PayOSClient payOSClient)
        {
            _checkoutService = checkoutService;
            _orderAdminService = orderAdminService;
            _orderRepo = orderRepo;
            _payOSClient = payOSClient;
        }

        [HttpPost("{orderId}/pay")]
        public async Task<IActionResult> ProcessCheckout(int orderId)
        {
            try
            {
                var url = await _checkoutService.CreatePaymentLinkAsync(orderId);
                return Ok(new { checkoutUrl = url });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ Gọi PayOS check status thực tế rồi update DB
        [HttpPost("{orderId}/confirm")]
        public async Task<IActionResult> ConfirmPayment(int orderId)
        {
            try
            {
                var paymentInfo = await _payOSClient.PaymentRequests.GetAsync(orderId);
                if (paymentInfo == null)
                    return Ok(new { success = false, message = "Không tìm thấy giao dịch." });

                var statusStr = paymentInfo.Status.ToString().ToUpper();

                if (statusStr == "PAID")
                {
                    // Check xem đã có giao dịch chưa để tránh duplicate
                    var totalPaid = await _orderRepo.GetTotalPaidAmountAsync(orderId);
                    if (totalPaid == 0)
                    {
                        await _orderAdminService.AddPaymentAsync(orderId, new AddPaymentDto
                        {
                            Amount = (decimal)paymentInfo.Amount,
                            PaymentMethod = "PayOS (QR Code)",
                            Status = "Success"
                        });
                    }
                    return Ok(new { success = true, status = "PAID" });
                }

                return Ok(new { success = false, status = statusStr });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}