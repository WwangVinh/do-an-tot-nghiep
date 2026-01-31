using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oto.MyModels;

namespace oto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly OtoContext _context;

        public UserController(OtoContext context)
        {
            _context = context;
        }

        // 1. GET: api/User
        // Lấy danh sách tất cả người dùng
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // 2. GET: api/User/5
        // Lấy thông tin chi tiết của một người dùng theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Airecommendations)
                .Include(u => u.CarWishlists)
                .Include(u => u.Orders)
                .Include(u => u.Reviews)
                .Include(u => u.UserActivities)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // 3. POST: api/User
        // Thêm mới một người dùng (đăng ký)
        [HttpPost]
        public async Task<ActionResult<User>> PostUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("Dữ liệu người dùng không hợp lệ.");
            }

            user.CreatedAt = DateTime.UtcNow; // Gán thời gian tạo tài khoản
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }

        // 4. PUT: api/User/5
        // Cập nhật thông tin người dùng theo ID
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, [FromBody] User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // 5. DELETE: api/User/5
        // Xóa một người dùng theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent(); // Trả về thành công sau khi xóa
        }

        // Kiểm tra nếu người dùng có tồn tại trong cơ sở dữ liệu
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
