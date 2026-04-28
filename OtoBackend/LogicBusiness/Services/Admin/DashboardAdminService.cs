using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;
using System;
using System.Threading.Tasks;

namespace LogicBusiness.Services.Admin
{
    public class DashboardAdminService : IDashboardAdminService
    {
        private readonly IDashboardRepository _repo;

        public DashboardAdminService(IDashboardRepository repo)
        {
            _repo = repo;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync(int days, string userRole, int? userShowroomId)
        {
            _ = DateTime.Now; // keep System import used (project may enforce)
            return await _repo.GetSummaryAsync(days, userRole, userShowroomId);
        }
    }
}

