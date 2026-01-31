using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oto.MyModels;

namespace oto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OtoContext _context;

        public OrderController(OtoContext context)
        {
            _context = context;
        }

        // 1. LẤY DANH SÁCH (Sắp xếp theo ID để đồng bộ với bảng của bạn)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            // Thêm .Include(o => o.Car) để lấy thông tin xe kèm theo đơn hàng
            return await _context.Orders
                .Include(o => o.Car)
                .ToListAsync();
        }

        // 2. TÌM KIẾM CHI TIẾT
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Order>>> SearchOrders(
            [FromQuery] int? userId,
            [FromQuery] string? status)
        {
            var query = _context.Orders
                .Include(o => o.Car)
                .Include(o => o.User)
                .AsQueryable();

            if (userId.HasValue)
                query = query.Where(o => o.UserId == userId);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status.Contains(status));

            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Car)
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();
            return Ok(order);
        }

        // 3. TẠO MỚI (Dùng FromForm giống CarController)
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(
            [FromForm] int userId,
            [FromForm] int carId,
            [FromForm] string? status,
            [FromForm] decimal? totalAmount,
            [FromForm] string? paymentMethod,
            [FromForm] string? shippingAddress)
        {
            // Kiểm tra ràng buộc để không bị lỗi 547
            if (!await _context.Users.AnyAsync(u => u.UserId == userId))
                return BadRequest("UserId không tồn tại.");

            if (!await _context.Cars.AnyAsync(c => c.CarId == carId))
                return BadRequest("CarId không tồn tại.");

            var order = new Order
            {
                UserId = userId,
                CarId = carId,
                OrderDate = DateTime.Now,
                Status = status ?? "Pending",
                TotalAmount = totalAmount ?? 0,
                PaymentMethod = paymentMethod,
                ShippingAddress = shippingAddress
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Load lại User/Car để Frontend có dữ liệu hiển thị ngay
            await _context.Entry(order).Reference(o => o.User).LoadAsync();
            await _context.Entry(order).Reference(o => o.Car).LoadAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
        }

        // 4. CẬP NHẬT
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id,
            [FromForm] int? userId,
            [FromForm] int? carId,
            [FromForm] string? status,
            [FromForm] decimal? totalAmount,
            [FromForm] string? paymentMethod,
            [FromForm] string? shippingAddress)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            if (userId.HasValue) order.UserId = userId;
            if (carId.HasValue) order.CarId = carId;
            order.Status = status ?? order.Status;
            order.TotalAmount = totalAmount ?? order.TotalAmount;
            order.PaymentMethod = paymentMethod ?? order.PaymentMethod;
            order.ShippingAddress = shippingAddress ?? order.ShippingAddress;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 5. XÓA
        // 5. DELETE: api/Order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            // Kiểm tra xem ID có hợp lệ không (phải lớn hơn 0)
            if (id <= 0)
            {
                return BadRequest("ID đơn hàng không hợp lệ.");
            }

            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound($"Không tìm thấy đơn hàng với ID {id} để xóa.");
            }

            try
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return NoContent(); // Xóa thành công trả về 204
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi khi xóa đơn hàng: " + ex.Message);
            }
        }
    }
}