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

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(int? userId, int? showroomId)
        {
            // Lấy thông báo cá nhân HOẶC thông báo chung của chi nhánh
            return await _context.Notifications
                .Where(n => (userId.HasValue && n.UserId == userId) ||
                            (showroomId.HasValue && n.ShowroomId == showroomId))
                .OrderByDescending(n => n.CreatedAt)
                .Take(50) // Chỉ lấy 50 cái mới nhất cho nhẹ web
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