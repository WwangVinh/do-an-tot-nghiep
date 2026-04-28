using CoreEntities.DTOs;
using LogicBusiness.Interfaces.Customer;
using Microsoft.AspNetCore.Mvc;

namespace OtoBackend.Controllers.Customer
{
    [Route("api/public/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // POST: api/public/orders/checkout
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CreateOrderDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Nhận kết quả từ Service
            var result = await _orderService.CreateGuestOrderAsync(request);

            // NẾU HẾT LƯỢT DÙNG MÃ KHUYẾN MÃI -> BÁO LỖI VỀ FE
            if (!result.Success)
            {
                return BadRequest(new { success = false, message = result.Message });
            }

            // THÀNH CÔNG
            return Ok(new
            {
                success = true,
                message = result.Message,
                orderCode = result.OrderCode
            });
        }

        // GET: api/public/orders/lookup?phone=0987654321&code=OTO-ABCD
        [HttpGet("lookup")]
        public async Task<IActionResult> Lookup([FromQuery] string phone, [FromQuery] string code)
        {
            if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(code))
                return BadRequest(new { success = false, message = "Vui lòng nhập SĐT và Mã đơn hàng" });

            var result = await _orderService.LookupOrderAsync(phone, code);

            if (result == null)
                return NotFound(new { success = false, message = "Không tìm thấy đơn hàng. Vui lòng kiểm tra lại thông tin." });

            return Ok(new { success = true, data = result });
        }
    }
}
