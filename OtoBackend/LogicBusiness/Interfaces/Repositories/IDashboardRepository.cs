using LogicBusiness.DTOs;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface IDashboardRepository
    {
        Task<DashboardSummaryDto> GetSummaryAsync(int days, string userRole, int? userShowroomId);
    }
}

