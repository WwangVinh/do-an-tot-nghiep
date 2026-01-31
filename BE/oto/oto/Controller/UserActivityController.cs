using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oto.MyModels;

namespace oto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserActivityController : ControllerBase
    {
        private readonly OtoContext _context;

        public UserActivityController(OtoContext context)
        {
            _context = context;
        }

        // 1. GET: api/UserActivity
        // Lấy danh sách tất cả hoạt động người dùng
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserActivity>>> GetUserActivities()
        {
            var activities = await _context.UserActivities
                .Include(ua => ua.User)  // Lấy thông tin người dùng liên quan đến hoạt động
                .Include(ua => ua.Car)   // Lấy thông tin xe liên quan đến hoạt động
                .ToListAsync();

            return Ok(activities);
        }

        // 2. GET: api/UserActivity/5
        // Lấy thông tin chi tiết của một hoạt động người dùng theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<UserActivity>> GetUserActivity(int id)
        {
            var userActivity = await _context.UserActivities
                .Include(ua => ua.User)  // Lấy thông tin người dùng liên quan đến hoạt động
                .Include(ua => ua.Car)   // Lấy thông tin xe liên quan đến hoạt động
                .FirstOrDefaultAsync(ua => ua.ActivityId == id);

            if (userActivity == null)
            {
                return NotFound();
            }

            return userActivity;
        }

        // 3. POST: api/UserActivity
        // Thêm một hoạt động người dùng mới
        [HttpPost]
        public async Task<ActionResult<UserActivity>> PostUserActivity([FromBody] UserActivity userActivity)
        {
            if (userActivity == null)
            {
                return BadRequest("Dữ liệu hoạt động người dùng không hợp lệ.");
            }

            userActivity.ActivityDate = DateTime.UtcNow; // Gán ngày hoạt động
            _context.UserActivities.Add(userActivity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserActivity), new { id = userActivity.ActivityId }, userActivity);
        }

        // 4. PUT: api/UserActivity/5
        // Cập nhật thông tin của một hoạt động người dùng theo ID
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserActivity(int id, [FromBody] UserActivity userActivity)
        {
            if (id != userActivity.ActivityId)
            {
                return BadRequest();
            }

            _context.Entry(userActivity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserActivityExists(id))
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

        // 5. DELETE: api/UserActivity/5
        // Xóa hoạt động người dùng theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserActivity(int id)
        {
            var userActivity = await _context.UserActivities.FindAsync(id);
            if (userActivity == null)
            {
                return NotFound();
            }

            _context.UserActivities.Remove(userActivity);
            await _context.SaveChangesAsync();

            return NoContent(); // Trả về thành công sau khi xóa
        }

        // Kiểm tra nếu hoạt động người dùng có tồn tại trong cơ sở dữ liệu
        private bool UserActivityExists(int id)
        {
            return _context.UserActivities.Any(e => e.ActivityId == id);
        }
    }
}
