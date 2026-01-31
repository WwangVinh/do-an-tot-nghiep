using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oto.MyModels;

namespace oto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly OtoContext _context;

        public ReviewController(OtoContext context)
        {
            _context = context;
        }

        // 1. GET: api/Review
        // Lấy danh sách tất cả đánh giá
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviews()
        {
            var reviews = await _context.Reviews
                .Include(r => r.Car) // Lấy thông tin xe liên quan đến đánh giá
                .Include(r => r.User) // Lấy thông tin người dùng liên quan đến đánh giá
                .ToListAsync();

            return Ok(reviews);
        }

        // 2. GET: api/Review/5
        // Lấy thông tin chi tiết của một đánh giá theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReview(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Car) // Lấy thông tin xe liên quan đến đánh giá
                .Include(r => r.User) // Lấy thông tin người dùng liên quan đến đánh giá
                .FirstOrDefaultAsync(r => r.ReviewId == id);

            if (review == null)
            {
                return NotFound();
            }

            return review;
        }

        // 3. POST: api/Review
        // Thêm mới một đánh giá cho xe
        [HttpPost]
        public async Task<ActionResult<Review>> PostReview([FromBody] Review review)
        {
            if (review == null)
            {
                return BadRequest("Dữ liệu đánh giá không hợp lệ.");
            }

            review.CreatedAt = DateTime.UtcNow; // Thêm ngày tạo cho đánh giá

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReview), new { id = review.ReviewId }, review);
        }

        // 4. PUT: api/Review/5
        // Cập nhật thông tin đánh giá theo ID
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReview(int id, [FromBody] Review review)
        {
            if (id != review.ReviewId)
            {
                return BadRequest();
            }

            _context.Entry(review).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Trả về thành công sau khi cập nhật
        }

        // 5. DELETE: api/Review/5
        // Xóa đánh giá theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent(); // Trả về thành công sau khi xóa
        }

        // Kiểm tra nếu đánh giá có tồn tại trong cơ sở dữ liệu
        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.ReviewId == id);
        }
    }
}
