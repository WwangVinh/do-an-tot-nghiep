using CoreEntities.Models.DTOs;
using LogicBusiness.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Admin
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<(bool Success, string Message)> HandleUserStatusAsync(int targetUserId, string action, int currentUserId, string currentUserRole, int? currentUserShowroomId);
        Task<object> GetFilteredUsersAsync(string userType, bool isDeleted, string? search, int page, int pageSize, string currentUserRole, int? currentUserShowroomId, int? filterShowroomId = null);

        Task<(bool Success, string Message)> CreateStaffAccountAsync(StaffAccountRequestDto request, string currentUserRole, int? currentUserShowroomId);
        Task<(bool Success, string Message)> UpdateStaffAccountAsync(int targetUserId, UserUpdateRequestDto request, string currentUserRole, int? currentUserShowroomId);
    }
}
