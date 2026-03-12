using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Customer;
using SqlServer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Services.Admin
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllActiveUsersAsync();

            // Map từ Model Entity sang DTO để giấu PasswordHash
            return users.Select(u => new UserResponseDto
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                Phone = u.Phone,
                Role = u.Role,
                Status = u.Status,
                AvatarUrl = u.AvatarUrl,
                CreatedAt = u.CreatedAt
            });
        }

        public async Task<bool> DeleteUserAsync(int userId, int? deletedByUserId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            // Nếu không tìm thấy hoặc user đã bị xóa trước đó rồi
            if (user == null || user.IsDeleted)
            {
                return false;
            }

            // XÓA MỀM (Soft Delete)
            user.IsDeleted = true;
            user.DeletedAt = DateTime.Now;
            user.DeletedBy = deletedByUserId;
            user.Status = "Inactive"; // Cập nhật trạng thái

            await _userRepository.UpdateUserAsync(user);
            return true;
        }

        public async Task<object> GetFilteredUsersAsync(bool isDeleted, string? search, int page, int pageSize)
        {
            var result = await _userRepository.GetFilteredUsersAsync(isDeleted, search, page, pageSize);

            var userDtos = result.Users.Select(u => new UserResponseDto
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                Phone = u.Phone,
                Role = u.Role,
                Status = u.Status,
                AvatarUrl = u.AvatarUrl,
                CreatedAt = u.CreatedAt,
                DeletedAt = u.DeletedAt // Trả về thêm ngày xóa để React hiển thị
            });

            // Trả về object giống hệt cấu trúc React đang chờ
            return new
            {
                Data = userDtos,
                TotalCount = result.TotalCount
            };
        }
    }
}
