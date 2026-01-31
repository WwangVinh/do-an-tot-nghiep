using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oto.MyModels;

namespace oto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private readonly OtoContext _context;

        public OrderItemController(OtoContext context)
        {
            _context = context;
        }

        // 1. LẤY DANH SÁCH
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems()
        {
            return await _context.OrderItems
                .Include(oi => oi.Car)
                .Include(oi => oi.Order)
                .ToListAsync();
        }

        // 2. LẤY CHI TIẾT THEO ID
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItem>> GetOrderItem(int id)
        {
            var orderItem = await _context.OrderItems
                .Include(oi => oi.Car)
                .Include(oi => oi.Order)
                .FirstOrDefaultAsync(oi => oi.OrderItemId == id);

            if (orderItem == null) return NotFound();
            return Ok(orderItem);
        }

        // 3. TẠO MỚI (Dùng FromForm từng trường giống OrderController)
        [HttpPost]
        public async Task<ActionResult<OrderItem>> PostOrderItem(
            [FromForm] int orderId,
            [FromForm] int carId,
            [FromForm] int quantity,
            [FromForm] decimal price)
        {
            // Kiểm tra ràng buộc khóa ngoại (Foreign Key)
            if (!await _context.Orders.AnyAsync(o => o.OrderId == orderId))
                return BadRequest("OrderId không tồn tại.");

            if (!await _context.Cars.AnyAsync(c => c.CarId == carId))
                return BadRequest("CarId không tồn tại.");

            var orderItem = new OrderItem
            {
                OrderId = orderId,
                CarId = carId,
                Quantity = quantity,
                Price = price
            };

            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();

            // Load lại thông tin Car để trả về cho Frontend hiển thị tên xe ngay
            await _context.Entry(orderItem).Reference(oi => oi.Car).LoadAsync();

            return CreatedAtAction(nameof(GetOrderItem), new { id = orderItem.OrderItemId }, orderItem);
        }

        // 4. CẬP NHẬT (Dùng FromForm từng trường)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderItem(int id,
            [FromForm] int? orderId,
            [FromForm] int? carId,
            [FromForm] int? quantity,
            [FromForm] decimal? price)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null) return NotFound();

            // Chỉ cập nhật nếu có giá trị truyền vào
            if (orderId.HasValue) orderItem.OrderId = orderId.Value;
            if (carId.HasValue) orderItem.CarId = carId.Value;
            if (quantity.HasValue) orderItem.Quantity = quantity.Value;
            if (price.HasValue) orderItem.Price = price.Value;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi khi cập nhật: " + ex.Message);
            }
        }

        // 5. XÓA
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null) return NotFound();

            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}