using CoreEntities.Models;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlServer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly OtoContext _context;

        public UserRepository(OtoContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> UserExistsAsync(string username, string email)
        {
            return await _context.Users.AnyAsync(u => u.Username == username || u.Email == email);
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllActiveUsersAsync()
        {
            return await _context.Users
                .Where(u => u.IsDeleted == false)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // BẢN NÂNG CẤP: LỌC USER CÓ PHÂN QUYỀN (Thay thế cho hàm GetFilteredUsersAsync cũ)
        public async Task<(IEnumerable<User> Users, int TotalCount)> GetFilteredUsersAdminAsync(
            string userType, bool isDeleted, string? search, int page, int pageSize,
            string currentUserRole, int? currentUserShowroomId, int? filterShowroomId = null)
        {
            var query = _context.Users.AsQueryable();

            // 1. Lọc theo trạng thái xóa (Thùng rác hay Không)
            query = query.Where(u => u.IsDeleted == isDeleted);

            // 2. TÁCH KHU VỰC & BẢO KÊ PHÂN QUYỀN 
            if (userType == "Staff")
            {
                // Chỉ bốc ra những ông là nhân sự (Sales hoặc Manager)
                query = query.Where(u => u.Role == "ShowroomManager" || u.Role == "ShowroomSales");

                // 👇 Nếu người đang xem là Manager -> ÉP CHỈ ĐƯỢC THẤY NHÂN VIÊN SHOWROOM MÌNH
                if (currentUserRole == "ShowroomManager" && currentUserShowroomId.HasValue)
                {
                    query = query.Where(u => u.ShowroomId == currentUserShowroomId.Value);
                }
                // 👇 THÊM CÁI NÀY NÈ: Nếu là Admin và có chọn Showroom từ Dropdown để lọc
                else if (filterShowroomId.HasValue && filterShowroomId.Value > 0)
                {
                    query = query.Where(u => u.ShowroomId == filterShowroomId.Value);
                }
            }
            else if (userType == "Customer")
            {
                // Chỉ bốc ra những ông là khách hàng
                query = query.Where(u => u.Role == "Customer");
            }
            // 3. Lọc theo từ khóa tìm kiếm (Search)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var kw = $"%{search.Trim()}%"; // Bọc % 2 đầu để tìm kiếm LIKE trong SQL

                query = query.Where(u =>
                    (u.Username != null && EF.Functions.Like(u.Username, kw)) ||
                    (u.Email != null && EF.Functions.Like(u.Email, kw)) ||
                    (u.FullName != null && EF.Functions.Like(u.FullName, kw))
                );
            }
            // 4. Đếm tổng số lượng (để React làm phân trang)
            int totalCount = await query.CountAsync();

            // 5. Phân trang (Pagination) và sắp xếp
            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        public async Task<IEnumerable<User>> GetStaffForChatAsync()
        {
            // Chỉ lấy những người có quyền Sales hoặc Manager và tài khoản đang Active
            return await _context.Users
                .Where(u => (u.Role == "ShowroomSales" || u.Role == "ShowroomManager") && u.Status == "Active")
                .ToListAsync();
        }

        public async Task HardDeleteUserAsync(User user)
        {
            _context.Users.Remove(user); // 👈 Tuyệt chiêu bứng gốc
            await _context.SaveChangesAsync();
        }
    }
}
