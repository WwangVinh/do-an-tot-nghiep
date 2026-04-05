using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Interfaces.Shared;
using LogicBusiness.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                CreatedAt = DateTime.Now,
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


            if (userId.HasValue)
            {
                // Bắn cho 1 người cụ thể
                await _hubContext.Clients.Group($"User_{userId.Value}").SendAsync("ReceiveNotification", notiDto);
            }
            else if (roleTarget == "Admin" && !showroomId.HasValue)
            {
                // Bắn cho Admin tổng
                await _hubContext.Clients.Group("Role_Admin").SendAsync("ReceiveNotification", notiDto);
            }
            else if (showroomId.HasValue && !string.IsNullOrEmpty(roleTarget))
            {
                // Bắn cho 1 Chức vụ tại 1 Showroom (Ví dụ: Manager của Showroom 2)
                await _hubContext.Clients.Group($"Showroom_{showroomId.Value}_Role_{roleTarget}").SendAsync("ReceiveNotification", notiDto);
            }
            else if (showroomId.HasValue && string.IsNullOrEmpty(roleTarget))
            {
                // Bắn Broadcast cho cả Showroom
                await _hubContext.Clients.Group($"Showroom_{showroomId.Value}").SendAsync("ReceiveNotification", notiDto);
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