using CoreEntities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(int? userId, int? showroomId);
        Task<Notification?> GetByIdAsync(int id);
        Task UpdateAsync(Notification notification);
    }
}
