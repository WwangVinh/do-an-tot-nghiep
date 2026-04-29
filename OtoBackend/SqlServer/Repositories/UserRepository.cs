using CoreEntities.Models;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlServer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly OtoContext _context;

        // Danh sách các chức vụ thuộc nhóm "Nhân sự" để truy vấn nhanh
        private readonly string[] _staffRoles =
        {
            "Manager", "Sales", "Technician", "Content", "Marketing", "ShowroomSales"
        };

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
                .Where(u => !u.IsDeleted)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetUserByPhoneAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return null;
            var p = phone.Trim();

            // Xử lý các đầu số 0, 84, +84 để tìm kiếm chính xác
            var candidates = new List<string> { p };
            if (p.StartsWith("+84") && p.Length > 3) candidates.Add("0" + p.Substring(3));
            if (p.StartsWith("84") && p.Length > 2) candidates.Add("0" + p.Substring(2));
            if (p.StartsWith("0") && p.Length > 1)
            {
                candidates.Add("+84" + p.Substring(1));
                candidates.Add("84" + p.Substring(1));
            }

            return await _context.Users.FirstOrDefaultAsync(u => u.Phone != null && candidates.Contains(u.Phone));
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // 🚀 LỌC USER CÓ PHÂN CẤP QUẢN LÝ (Admin & Manager)
        public async Task<(IEnumerable<User> Users, int TotalCount)> GetFilteredUsersAdminAsync(
            string userType, bool isDeleted, string? search, int page, int pageSize,
            string currentUserRole, int? currentUserShowroomId, int? filterShowroomId = null)
        {
            var query = _context.Users.AsQueryable();

            // 1. Lọc theo trạng thái xóa (Thùng rác hay không)
            query = query.Where(u => u.IsDeleted == isDeleted);

            // 2. Phân loại theo nhóm đối tượng (Staff vs Customer)
            if (userType == "Staff")
            {
                // Chỉ lấy những tài khoản có chức vụ nằm trong danh sách Staff
                query = query.Where(u => _staffRoles.Contains(u.Role));

                // CHỐT CHẶN PHÂN CẤP:
                string safeRole = currentUserRole?.Trim().ToLower() ?? "";

                // Nếu là Quản lý chi nhánh -> Chỉ được thấy quân mình
                if (safeRole == "manager" && currentUserShowroomId.HasValue)
                {
                    query = query.Where(u => u.ShowroomId == currentUserShowroomId.Value);
                }
                // Nếu là Admin -> Được xem tất cả hoặc lọc theo showroom tùy chọn
                else if (safeRole == "admin" && filterShowroomId.HasValue && filterShowroomId.Value > 0)
                {
                    query = query.Where(u => u.ShowroomId == filterShowroomId.Value);
                }
            }
            else if (userType == "Customer")
            {
                query = query.Where(u => u.Role == "Customer");
            }

            // 3. Tìm kiếm theo từ khóa (LIKE)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var kw = $"%{search.Trim()}%";
                query = query.Where(u =>
                    (u.Username != null && EF.Functions.Like(u.Username, kw)) ||
                    (u.Email != null && EF.Functions.Like(u.Email, kw)) ||
                    (u.FullName != null && EF.Functions.Like(u.FullName, kw)) ||
                    (u.Phone != null && EF.Functions.Like(u.Phone, kw))
                );
            }

            int totalCount = await query.CountAsync();

            // 4. Phân trang và Sắp xếp
            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        // Lấy danh sách nhân sự hỗ trợ Chat (Bao gồm cả các chức vụ mới)
        public async Task<IEnumerable<User>> GetStaffForChatAsync()
        {
            return await _context.Users
                .Where(u => _staffRoles.Contains(u.Role) && u.Status == "Active")
                .ToListAsync();
        }

        public async Task HardDeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}