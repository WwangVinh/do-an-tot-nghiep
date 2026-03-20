using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Customer;
using LogicBusiness.Interfaces.Repositories;
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
        private readonly IShowroomRepository _showroomRepository;

        public UserService(IUserRepository userRepository, IShowroomRepository showroomRepository)
        {
            _userRepository = userRepository;
            _showroomRepository = showroomRepository;
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

        public async Task<(bool Success, string Message)> CreateStaffAccountAsync(StaffAccountRequestDto request)
        {
            // 1. NGHIỆP VỤ 1: Kiểm tra Role có hợp lệ không (Chống hách bằng Postman)
            var validRoles = new List<string> { "ShowroomManager", "ShowroomSales" };
            if (!validRoles.Contains(request.Role))
            {
                return (false, "Quyền (Role) không hợp lệ. Chỉ chấp nhận Manager hoặc Sales.");
            }

            // 2. NGHIỆP VỤ 2: Kiểm tra Username có bị trùng không
            var existingUser = await _userRepository.GetUserByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                return (false, "Tên đăng nhập này đã có người sử dụng!");
            }

            // 3. NGHIỆP VỤ 3: Kiểm tra xem ID Showroom Admin truyền vào có thật không
            var showroom = await _showroomRepository.GetByIdAsync(request.ShowroomId);
            if (showroom == null)
            {
                return (false, "Showroom không tồn tại trong hệ thống!");
            }

            // 4. XỬ LÝ DỮ LIỆU: Băm (Hash) mật khẩu bằng thư viện BCrypt
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 5. GÁN DỮ LIỆU: Map từ DTO sang Entity thực tế
            var newUser = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Role = request.Role,
                ShowroomId = request.ShowroomId, // Chìa khóa quan trọng nhất ở đây
                Status = "Active",
                CreatedAt = DateTime.Now
            };

            // 6. LƯU VÀO DB
            await _userRepository.AddUserAsync(newUser);

            return (true, $"Tạo tài khoản {request.Role} cho {request.FullName} tại chi nhánh {showroom.Name} thành công!");
        }
    }
}
