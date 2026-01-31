using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oto.MyModels;

namespace oto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirecommendationController : ControllerBase
    {
        private readonly OtoContext _context;

        public AirecommendationController(OtoContext context)
        {
            _context = context;
        }

        // 1. GET: api/Airecommendation
        // Lấy danh sách tất cả khuyến nghị AI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Airecommendation>>> GetAirecommendations()
        {
            var recommendations = await _context.Airecommendations
                .Include(ar => ar.Car)  // Lấy thông tin xe liên quan đến khuyến nghị
                .Include(ar => ar.User) // Lấy thông tin người dùng liên quan đến khuyến nghị
                .ToListAsync();

            return Ok(recommendations);
        }

        // 2. GET: api/Airecommendation/5
        // Lấy thông tin chi tiết của một khuyến nghị AI theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Airecommendation>> GetAirecommendation(int id)
        {
            var recommendation = await _context.Airecommendations
                .Include(ar => ar.Car)  // Lấy thông tin xe liên quan đến khuyến nghị
                .Include(ar => ar.User) // Lấy thông tin người dùng liên quan đến khuyến nghị
                .FirstOrDefaultAsync(ar => ar.RecommendationId == id);

            if (recommendation == null)
            {
                return NotFound();
            }

            return recommendation;
        }

        // 3. POST: api/Airecommendation
        // Thêm mới một khuyến nghị AI cho xe
        [HttpPost]
        public async Task<ActionResult<Airecommendation>> PostAirecommendation([FromBody] Airecommendation airecommendation)
        {
            if (airecommendation == null)
            {
                return BadRequest("Dữ liệu khuyến nghị AI không hợp lệ.");
            }

            airecommendation.CreatedAt = DateTime.UtcNow; // Gán ngày tạo cho khuyến nghị AI
            _context.Airecommendations.Add(airecommendation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAirecommendation), new { id = airecommendation.RecommendationId }, airecommendation);
        }

        // 4. PUT: api/Airecommendation/5
        // Cập nhật thông tin khuyến nghị AI theo ID
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAirecommendation(int id, [FromBody] Airecommendation airecommendation)
        {
            if (id != airecommendation.RecommendationId)
            {
                return BadRequest();
            }

            _context.Entry(airecommendation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AirecommendationExists(id))
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

        // 5. DELETE: api/Airecommendation/5
        // Xóa khuyến nghị AI theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAirecommendation(int id)
        {
            var airecommendation = await _context.Airecommendations.FindAsync(id);
            if (airecommendation == null)
            {
                return NotFound();
            }

            _context.Airecommendations.Remove(airecommendation);
            await _context.SaveChangesAsync();

            return NoContent(); // Trả về thành công sau khi xóa
        }

        // Kiểm tra nếu khuyến nghị AI có tồn tại trong cơ sở dữ liệu
        private bool AirecommendationExists(int id)
        {
            return _context.Airecommendations.Any(e => e.RecommendationId == id);
        }
    }
}
