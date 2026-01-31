using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oto.MyModels;

namespace oto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentTransactionController : ControllerBase
    {
        private readonly OtoContext _context;

        public PaymentTransactionController(OtoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetPaymentTransactions()
        {
            // Dùng Select để định hình lại dữ liệu trả về, tránh lỗi vòng lặp tham chiếu
            var transactions = await _context.PaymentTransactions
                .Include(pt => pt.Order)
                .Select(pt => new {
                    pt.TransactionId,
                    pt.OrderId,
                    pt.Amount,
                    pt.PaymentMethod,
                    pt.TransactionDate,
                    pt.Status,
                    // Nếu bạn tìm thấy tên cột trong Order.cs (VD: CustomerName), hãy thay vào đây:
                    OrderInfo = pt.Order != null ? "Đơn hàng #" + pt.Order.OrderId : "N/A"
                })
                .ToListAsync();

            return Ok(transactions);
        }

        // 2. GET: api/PaymentTransaction/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentTransaction>> GetPaymentTransaction(int id)
        {
            var transaction = await _context.PaymentTransactions
                .Include(pt => pt.Order)
                .FirstOrDefaultAsync(pt => pt.TransactionId == id);

            if (transaction == null) return NotFound();

            return transaction;
        }

        // 3. POST: api/PaymentTransaction
        // Chuyển sang nhận tham số trực tiếp để dễ dàng debug lỗi 400
        [HttpPost]
        public async Task<ActionResult<PaymentTransaction>> PostPaymentTransaction([FromForm] int orderId, [FromForm] decimal amount, [FromForm] string paymentMethod, [FromForm] string status)
        {
            // Kiểm tra OrderId có tồn tại không để tránh lỗi 547
            var orderExists = await _context.Orders.AnyAsync(o => o.OrderId == orderId);
            if (!orderExists)
            {
                return BadRequest($"Không tìm thấy đơn hàng mã #{orderId}");
            }

            var transaction = new PaymentTransaction
            {
                OrderId = orderId,
                Amount = amount,
                PaymentMethod = paymentMethod,
                Status = status,
                TransactionDate = DateTime.Now // Gán giờ hiện tại
            };

            _context.PaymentTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPaymentTransaction), new { id = transaction.TransactionId }, transaction);
        }

        // 4. PUT: api/PaymentTransaction/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaymentTransaction(int id, [FromForm] string status)
        {
            var transaction = await _context.PaymentTransactions.FindAsync(id);
            if (transaction == null) return NotFound();

            // Thường người ta chỉ cho phép sửa Trạng thái giao dịch
            transaction.Status = status;

            _context.Entry(transaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentTransactionExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // 5. DELETE: api/PaymentTransaction/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaymentTransaction(int id)
        {
            var paymentTransaction = await _context.PaymentTransactions.FindAsync(id);
            if (paymentTransaction == null) return NotFound();

            _context.PaymentTransactions.Remove(paymentTransaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PaymentTransactionExists(int id)
        {
            return _context.PaymentTransactions.Any(e => e.TransactionId == id);
        }
    }
}