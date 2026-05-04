using LogicBusiness.Interfaces.Admin;
using Microsoft.AspNetCore.Mvc;
using PayOS;
using PayOS.Models;
using PayOS.Models.Webhooks;
using System;
using System.Threading.Tasks;

namespace OtoBackend.Controllers
{
    [Route("api/payos-webhook")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly PayOSClient _payOSClient;
        private readonly IOrderAdminService _orderAdminService;

        public WebhookController(PayOSClient payOSClient, IOrderAdminService orderAdminService)
        {
            _payOSClient = payOSClient;
            _orderAdminService = orderAdminService;
        }

        [HttpPost]
        public async Task<IActionResult> HandleWebhook([FromBody] PayOS.Models.Webhooks.Webhook webhookBody)
        {
            try
            {
                // Truyền webhookBody (kiểu Webhook) vào để Verify thay vì WebhookData
                var verifiedData = await _payOSClient.Webhooks.VerifyAsync(webhookBody);

                int orderId = (int)verifiedData.OrderCode;

                var paymentDto = new LogicBusiness.DTOs.AddPaymentDto
                {
                    Amount = verifiedData.Amount,
                    PaymentMethod = "PayOS (QR Code)",
                    Status = "Success"
                };

                await _orderAdminService.AddPaymentAsync(orderId, paymentDto);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}