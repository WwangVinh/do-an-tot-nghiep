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

            var result = await _orderService.CreateGuestOrderAsync(request);

            if (!result.Success)
                return BadRequest(new { success = false, message = result.Message });

            return Ok(new
            {
                success = true,
                message = result.Message,
                orderCode = result.OrderCode,
                orderId = result.OrderId
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

        // ✅ GET: api/public/orders/by-phone?phone=0987654321
        // Lấy tất cả đơn hàng theo SĐT — dùng cho tra cứu nhanh trên Header
        [HttpGet("by-phone")]
        public async Task<IActionResult> GetByPhone([FromQuery] string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return BadRequest(new { message = "Vui lòng nhập số điện thoại." });

            var orders = await _orderService.GetOrdersByPhoneAsync(phone.Trim());

            if (orders == null || !orders.Any())
                return NotFound(new { message = "Số điện thoại này chưa có đơn hàng nào." });

            return Ok(orders);
        }

        [HttpGet("check-promotion")]
        public async Task<IActionResult> CheckPromotion([FromQuery] string code, [FromQuery] int carId)
        {
            if (string.IsNullOrWhiteSpace(code))
                return BadRequest(new { message = "Vui lòng nhập mã giảm giá!" });

            var result = await _orderService.CheckPromotionAsync(code, carId);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new
            {
                discountPercentage = result.DiscountPercentage,
                message = result.Message
            });
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _orderService.CancelOrderAsync(id);
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        // GET: api/public/orders/showrooms
        // Trả về danh sách showroom để khách chọn khi đặt xe
        [HttpGet("showrooms")]
        public async Task<IActionResult> GetShowrooms()
        {
            var showrooms = await _orderService.GetAvailableShowroomsAsync();
            return Ok(new { success = true, data = showrooms });
        }
        // GET: api/public/cars/{carId}/showrooms
        [HttpGet("~/api/public/cars/{carId}/showrooms")]
        public async Task<IActionResult> GetShowroomsByCar(int carId)
        {
            var showrooms = await _orderService.GetShowroomsByCarIdAsync(carId);
            return Ok(new { success = true, data = showrooms });
        }
    }
}