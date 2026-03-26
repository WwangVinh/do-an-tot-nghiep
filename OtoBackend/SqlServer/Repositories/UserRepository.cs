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

        public async Task<(IEnumerable<User> Users, int TotalCount)> GetFilteredUsersAsync(bool isDeleted, string? search, int page, int pageSize)
        {
            var query = _context.Users.AsQueryable();

            // 1. Lọc theo trạng thái xóa (Thùng rác hay Không)
            query = query.Where(u => u.IsDeleted == isDeleted);

            // 2. Lọc theo từ khóa tìm kiếm (Search)
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u => u.Username.Contains(search)
                                      || u.Email.Contains(search)
                                      || u.FullName.Contains(search));
            }

            // Đếm tổng số lượng (để React làm phân trang)
            int totalCount = await query.CountAsync();

            // 3. Phân trang (Pagination) và sắp xếp
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
    }
}
