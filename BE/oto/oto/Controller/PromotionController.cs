using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oto.MyModels;

namespace oto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly OtoContext _context;

        public PromotionController(OtoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Promotion>>> GetPromotions()
        {
            return await _context.Promotions.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Promotion>> GetPromotion(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null) return NotFound();
            return promotion;
        }

        // 3. POST: Thêm mới
        [HttpPost]
        public async Task<ActionResult<Promotion>> PostPromotion(
            [FromForm] string code,
            [FromForm] decimal discountPercentage,
            [FromForm] DateTime startDate,
            [FromForm] DateTime endDate,
            [FromForm] string? description,
            [FromForm] int status) // Nhận 0 hoặc 1 từ giao diện
        {
            if (await _context.Promotions.AnyAsync(p => p.Code == code))
                return BadRequest("Mã khuyến mãi này đã tồn tại.");

            var promotion = new Promotion
            {
                Code = code,
                DiscountPercentage = discountPercentage,
                StartDate = startDate,
                EndDate = endDate,
                Description = description,
                // SỬA LỖI CS0029: Chuyển từ số sang chuỗi để khớp với kiểu string trong Model
                Status = status == 1 ? "Active" : "Inactive"
            };

            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPromotion), new { id = promotion.PromotionId }, promotion);
        }

        // 4. PUT: Cập nhật
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPromotion(int id,
            [FromForm] string? code,
            [FromForm] decimal? discountPercentage,
            [FromForm] DateTime? startDate,
            [FromForm] DateTime? endDate,
            [FromForm] string? description,
            [FromForm] int? status)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null) return NotFound();

            if (!string.IsNullOrEmpty(code) && code != promotion.Code)
            {
                if (await _context.Promotions.AnyAsync(p => p.Code == code && p.PromotionId != id))
                    return BadRequest("Mã khuyến mãi mới đã tồn tại.");
                promotion.Code = code;
            }

            if (discountPercentage.HasValue) promotion.DiscountPercentage = discountPercentage.Value;
            if (startDate.HasValue) promotion.StartDate = startDate.Value;
            if (endDate.HasValue) promotion.EndDate = endDate.Value;

            // SỬA LỖI CS0029: Cập nhật status kiểu string
            if (status.HasValue)
            {
                promotion.Status = status.Value == 1 ? "Active" : "Inactive";
            }

            promotion.Description = description ?? promotion.Description;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi: " + ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePromotion(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null) return NotFound();

            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}