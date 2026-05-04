using CoreEntities.Models;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlServer.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly OtoContext _context;
        public NotificationRepository(OtoContext context) => _context = context;

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(int? userId, int? showroomId, string? userRole)
        {
            // Lấy toàn bộ rồi lọc phía application vì RoleTarget có thể là "Manager,Sales,ShowroomSales"
            var all = await _context.Notifications
                .OrderByDescending(n => n.CreatedAt)
                .Take(200)
                .ToListAsync();

            return all.Where(n =>
                // 1. Tin nhắn cá nhân gửi đích danh
                (userId.HasValue && n.UserId == userId) ||

                // 2. Broadcast toàn showroom (không target role cụ thể)
                (showroomId.HasValue && n.ShowroomId == showroomId && n.UserId == null && string.IsNullOrEmpty(n.RoleTarget)) ||

                // 3. Tin nhắn cho role tại showroom — hỗ trợ multi-role "Manager,Sales,ShowroomSales"
                (showroomId.HasValue && n.ShowroomId == showroomId && !string.IsNullOrEmpty(n.RoleTarget) && !string.IsNullOrEmpty(userRole) &&
                    n.RoleTarget.Split(',').Select(r => r.Trim()).Contains(userRole)) ||

                // 4. Tin nhắn global cho role (không ràng buộc showroom)
                (n.ShowroomId == null && n.UserId == null && !string.IsNullOrEmpty(n.RoleTarget) && !string.IsNullOrEmpty(userRole) &&
                    n.RoleTarget.Split(',').Select(r => r.Trim()).Contains(userRole))
            )
            .OrderByDescending(n => n.CreatedAt)
            .Take(50);
        }

        public async Task<Notification?> GetByIdAsync(int id) => await _context.Notifications.FindAsync(id);

        public async Task UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }
    }
}