using LogicBusiness.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Shared
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(int? userId, int? showroomId, string title, string content, string actionUrl, string type);
        Task<IEnumerable<NotificationDto>> GetMyNotificationsAsync(int? userId, int? showroomId);
        Task<bool> MarkAsReadAsync(int notificationId);
    }
}