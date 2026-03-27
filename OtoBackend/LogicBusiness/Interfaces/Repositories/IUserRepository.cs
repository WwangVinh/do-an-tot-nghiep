using CoreEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<bool> UserExistsAsync(string username, string email);
        Task AddUserAsync(User user);

        Task<IEnumerable<User>> GetAllActiveUsersAsync();
        Task<User?> GetUserByIdAsync(int userId);
        Task UpdateUserAsync(User user);
        Task<(IEnumerable<User> Users, int TotalCount)> GetFilteredUsersAsync(bool isDeleted, string? search, int page, int pageSize);

        Task<IEnumerable<User>> GetStaffForChatAsync();

        //uqwuieuqwo

    }
}
