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
            return await _context.Notifications
                .Where(n =>
                    // Tin nhắn cá nhân gửi đích danh (Có UserId)
                    (userId.HasValue && n.UserId == userId) ||

                    // Tin nhắn cho một Showroom cụ thể (Ai trong Showroom cũng thấy)
                    (showroomId.HasValue && n.ShowroomId == showroomId && n.UserId == null && n.RoleTarget == null) ||

                    // Tin nhắn công việc cho một Chức vụ cụ thể TẠI MỘT Showroom
                    (showroomId.HasValue && n.ShowroomId == showroomId && n.RoleTarget == userRole) ||

                    // Tin nhắn công việc cho một Chức vụ cụ thể TOÀN CỤC (Không ràng buộc Showroom)
                    (n.ShowroomId == null && n.RoleTarget == userRole && n.UserId == null)
                )
                .OrderByDescending(n => n.CreatedAt)
                .Take(50)
                .ToListAsync();
        }

        public async Task<Notification?> GetByIdAsync(int id) => await _context.Notifications.FindAsync(id);

        public async Task UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }
    }
}