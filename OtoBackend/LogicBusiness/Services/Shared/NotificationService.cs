using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Interfaces.Shared;

namespace LogicBusiness.Services.Shared
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(INotificationRepository repo, IHubContext<NotificationHub> hubContext)
        {
            _repo = repo;
            _hubContext = hubContext;
        }

        public async Task CreateNotificationAsync(int? userId, int? showroomId, string? roleTarget, string title, string content, string actionUrl, string type)
        {
            var noti = new Notification
            {
                UserId = userId,
                ShowroomId = showroomId,
                RoleTarget = roleTarget,
                Title = title,
                Content = content,
                ActionUrl = actionUrl,
                NotificationType = type,
                // Fix chuẩn múi giờ Việt Nam (+7) bất chấp server đặt ở đâu
                CreatedAt = DateTime.UtcNow.AddHours(7),
                IsRead = false
            };
            await _repo.AddAsync(noti);

            var notiDto = new NotificationDto
            {
                NotificationId = noti.NotificationId,
                Title = noti.Title,
                Content = noti.Content,
                ActionUrl = noti.ActionUrl,
                NotificationType = noti.NotificationType,
                IsRead = noti.IsRead,
                CreatedAt = noti.CreatedAt.ToString("dd/MM/yyyy HH:mm")
            };

            // LOGIC PHÂN LUỒNG SIGNALR ĐÃ ĐƯỢC TỐI ƯU HÓA:
            if (userId.HasValue)
            {
                // 1. Bắn cho 1 người cụ thể (Ví dụ: Thông báo đơn hàng cho khách)
                await _hubContext.Clients.Group($"User_{userId.Value}").SendAsync("ReceiveNotification", notiDto);
            }
            else if (!showroomId.HasValue && !string.IsNullOrEmpty(roleTarget))
            {
                // 2. Bắn cho 1 Role chung toàn hệ thống (VD: Admin, Marketing, Sales tổng...)
                // Tự động linh hoạt không cần code cứng chữ "Admin"
                await _hubContext.Clients.Group($"Role_{roleTarget}").SendAsync("ReceiveNotification", notiDto);
            }
            else if (showroomId.HasValue && !string.IsNullOrEmpty(roleTarget))
            {
                // 3. Bắn cho 1 Chức vụ tại 1 Showroom (Ví dụ: ShowroomSales của Showroom 2)
                await _hubContext.Clients.Group($"Showroom_{showroomId.Value}_Role_{roleTarget}").SendAsync("ReceiveNotification", notiDto);
            }
            else if (showroomId.HasValue && string.IsNullOrEmpty(roleTarget))
            {
                // 4. Bắn Broadcast cho toàn bộ nhân viên thuộc 1 Showroom
                await _hubContext.Clients.Group($"Showroom_{showroomId.Value}").SendAsync("ReceiveNotification", notiDto);
            }
            else
            {
                // 5. Bắn Broadcast cho TOÀN BỘ hệ thống (Trường hợp cả userId, showroomId, roleTarget đều null)
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", notiDto);
            }
        }

        public async Task<IEnumerable<NotificationDto>> GetMyNotificationsAsync(int? userId, int? showroomId, string? userRole)
        {
            var notis = await _repo.GetUserNotificationsAsync(userId, showroomId, userRole);
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