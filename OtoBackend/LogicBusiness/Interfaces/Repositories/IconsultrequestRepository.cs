using CoreEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface IConsultRequestRepository
    {
        Task<ConsultRequest?> GetByIdAsync(int id);
        Task<IEnumerable<ConsultRequest>> GetByPhoneAsync(string phone);
        Task AddAsync(ConsultRequest entity);
        Task UpdateAsync(ConsultRequest entity);

        Task<(IEnumerable<ConsultRequest> Items, int Total)> GetAdminListAsync(
            int page, int pageSize,
            string? search, string? status, string? requestType,
            int? showroomId, int? assignedUserId);

        Task<Dictionary<string, int>> CountByStatusAsync(int? showroomId);
    }
}
