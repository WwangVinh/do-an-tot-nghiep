using LogicBusiness.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Admin
{
    public interface IConsultRequestAdminService
    {
        Task<object> GetListAsync(
            int page, int pageSize, string? search, string? status, string? requestType,
            string userRole, int? userShowroomId, int currentUserId);

        Task<object?> GetDetailAsync(int id, string userRole, int? userShowroomId);

        Task<(bool Success, string Message)> PickupAsync(
            int id, ConsultPickupDto dto, int currentUserId, string userRole, int? userShowroomId);

        Task<(bool Success, string Message)> SubmitResultAsync(
            int id, ConsultResultDto dto, int currentUserId, string userRole, int? userShowroomId);

        Task<(bool Success, string Message)> CancelByAdminAsync(
            int id, string cancelReason, int currentUserId, string userRole, int? userShowroomId);

        Task<Dictionary<string, int>> GetStatsAsync(string userRole, int? userShowroomId);
    }
}
