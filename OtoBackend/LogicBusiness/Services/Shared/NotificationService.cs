using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Interfaces.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicBusiness.Services.Shared
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;

        public NotificationService(INotificationRepository repo) => _repo = repo;

        // Hàm này dùng để gọi nội bộ từ các Service khác (như CarAdminService)
        public async Task CreateNotificationAsync(int? userId, int? showroomId, string title, string content, string actionUrl, string type)
        {
            var noti = new Notification
            {
                UserId = userId,
                ShowroomId = showroomId,
                Title = title,
                Content = content,
                ActionUrl = actionUrl,
                NotificationType = type, // "CarApproval", "Booking", "Chat"...
                CreatedAt = DateTime.Now,
                IsRead = false
            };
            await _repo.AddAsync(noti);

            // 👉 NẾU CÓ SIGNALR: Ní gọi cái Hub context ở đây để bắn thông báo real-time lên FE luôn!
        }

        public async Task<IEnumerable<NotificationDto>> GetMyNotificationsAsync(int? userId, int? showroomId)
        {
            var notis = await _repo.GetUserNotificationsAsync(userId, showroomId);
            return notis.Select(n => new NotificationDto
            {
                NotificationId = n.NotificationId,
                Title = n.Title,
                Content = n.Content,
                ActionUrl = n.ActionUrl,
                NotificationType = n.NotificationType,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt.ToString("dd/MM/yyyy HH:mm")
            });
        }

        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            var noti = await _repo.GetByIdAsync(notificationId);
            if (noti == null) return false;

            noti.IsRead = true;
            await _repo.UpdateAsync(noti);
            return true;
        }
    }
}