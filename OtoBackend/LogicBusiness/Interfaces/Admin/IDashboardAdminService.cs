using LogicBusiness.DTOs;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Admin
{
    public interface IDashboardAdminService
    {
        Task<DashboardSummaryDto> GetSummaryAsync(int days, string userRole, int? userShowroomId);
    }
}

