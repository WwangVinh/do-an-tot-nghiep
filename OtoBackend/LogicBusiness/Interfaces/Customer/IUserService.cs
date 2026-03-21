using LogicBusiness.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Customer
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<bool> DeleteUserAsync(int userId, int? deletedByUserId);
        Task<object> GetFilteredUsersAsync(bool isDeleted, string? search, int page, int pageSize);

        Task<(bool Success, string Message)> CreateStaffAccountAsync(StaffAccountRequestDto request);
    }
}
