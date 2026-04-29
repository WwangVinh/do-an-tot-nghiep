using CoreEntities.Models;
using CoreEntities.Models.DTOs;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Interfaces.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicBusiness.Services.Admin
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IShowroomRepository _showroomRepository;
        private readonly INotificationService _notiService;

        // Roles nhân viên cấp dưới — Manager được tạo/sửa
        private readonly List<string> _staffRoles = new List<string>
        {
            AppRoles.Sales, AppRoles.Technician, AppRoles.Content,
            AppRoles.Marketing, AppRoles.ShowroomSales
        };

        public UserService(IUserRepository userRepository, IShowroomRepository showroomRepository, INotificationService notiService)
        {
            _userRepository = userRepository;
            _showroomRepository = showroomRepository;
            _notiService = notiService;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllActiveUsersAsync();
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

        public async Task<object> GetFilteredUsersAsync(
            string userType, bool isDeleted, string? search, int page, int pageSize,
            string currentUserRole, int? currentUserShowroomId,
            int? filterShowroomId = null)
        {
            var result = await _userRepository.GetFilteredUsersAdminAsync(
                userType, isDeleted, search, page, pageSize, currentUserRole, currentUserShowroomId, filterShowroomId);

            var userDtos = result.Users.Select(u => new UserResponseDto
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                Phone = u.Phone,
                Role = u.Role,
                Status = u.Status,
                ShowroomId = u.ShowroomId,
                AvatarUrl = u.AvatarUrl,
                CreatedAt = u.CreatedAt,
                DeletedAt = u.DeletedAt
            });

            return new { Data = userDtos, TotalCount = result.TotalCount };
        }

        public async Task<(bool Success, string Message)> CreateStaffAccountAsync(
            StaffAccountRequestDto request, string currentUserRole, int? currentUserShowroomId)
        {
            string safeRole = currentUserRole?.Trim() ?? "";

            if (safeRole == AppRoles.Manager)
            {
                if (!_staffRoles.Contains(request.Role))
                    return (false, "Quản lý chỉ được tạo tài khoản nhân viên cấp dưới!");

                if (request.ShowroomId != currentUserShowroomId)
                    return (false, "Quản lý chỉ được tạo nhân viên trong chi nhánh của mình!");
            }
            else if (safeRole != AppRoles.Admin)
            {
                return (false, "Bạn không có quyền thực hiện chức năng này!");
            }

            var allValidRoles = new List<string>(_staffRoles) { AppRoles.Manager, AppRoles.Admin };
            if (!allValidRoles.Contains(request.Role))
                return (false, $"Role '{request.Role}' không hợp lệ trong hệ thống!");

            var existingUser = await _userRepository.GetUserByUsernameAsync(request.Username);
            if (existingUser != null)
                return (false, "Tên đăng nhập này đã có người sử dụng!");

            var showroom = await _showroomRepository.GetByIdAsync(request.ShowroomId);
            if (showroom == null)
                return (false, "Showroom không tồn tại!");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Role = request.Role,
                ShowroomId = request.ShowroomId,
                Status = "Active",
                CreatedAt = DateTime.Now
            };

            await _userRepository.AddUserAsync(newUser);

            await _notiService.CreateNotificationAsync(
                userId: null,
                roleTarget: null,
                showroomId: request.ShowroomId,
                title: "Nhân sự mới gia nhập! 🎉",
                content: $"Chào mừng {request.FullName} ({request.Role}) vừa gia nhập chi nhánh.",
                actionUrl: "/admin/users",
                type: "System"
            );

            return (true, $"Tạo tài khoản {request.Role} thành công cho {request.FullName}!");
        }

        public async Task<(bool Success, string Message)> HandleUserStatusAsync(
            int targetUserId, string action, int currentUserId,
            string currentUserRole, int? currentUserShowroomId)
        {
            var user = await _userRepository.GetUserByIdAsync(targetUserId);
            if (user == null || user.IsDeleted)
                return (false, "Không tìm thấy người dùng hoặc tài khoản đã bị xóa!");

            if (user.UserId == currentUserId)
                return (false, "Không thể tự khóa tài khoản của chính mình!");

            string safeAction = action?.Trim().ToLower() ?? "";
            string safeRole = currentUserRole?.Trim() ?? "";

            if (safeRole == AppRoles.Manager)
            {
                if (user.Role == AppRoles.Admin || user.Role == AppRoles.Manager)
                    return (false, "Quản lý không có quyền thao tác lên cấp trên hoặc đồng cấp!");

                if (!currentUserShowroomId.HasValue || user.ShowroomId != currentUserShowroomId.Value)
                    return (false, "Nhân viên này thuộc chi nhánh khác!");

                if (safeAction == "delete" || safeAction == "hard_delete")
                    return (false, "Quản lý chỉ có quyền Khóa/Mở khóa. Việc xóa tài khoản do Admin thực hiện!");

                if (new[] { "deactivate", "inactive", "lock" }.Contains(safeAction))
                    user.Status = "Inactive";
                else if (new[] { "activate", "active", "unlock" }.Contains(safeAction))
                    user.Status = "Active";
                else
                    return (false, $"Hành động '{action}' không hợp lệ!");
            }
            else if (safeRole == AppRoles.Admin)
            {
                if (safeAction == "delete")
                {
                    user.IsDeleted = true;
                    user.DeletedAt = DateTime.Now;
                    user.DeletedBy = currentUserId;
                    user.Status = "Inactive";
                }
                else if (safeAction == "hard_delete")
                {
                    try
                    {
                        string deletedName = user.FullName;
                        int? showroomId = user.ShowroomId;
                        await _userRepository.HardDeleteUserAsync(user);

                        if (showroomId.HasValue)
                        {
                            await _notiService.CreateNotificationAsync(
                                userId: null,
                                showroomId: showroomId.Value,
                                roleTarget: AppRoles.Manager,
                                title: "Tài khoản bị xóa vĩnh viễn ☠️",
                                content: $"Admin vừa xóa vĩnh viễn tài khoản {deletedName} khỏi hệ thống.",
                                actionUrl: "/admin/users",
                                type: "System"
                            );
                        }
                        return (true, "Đã xóa vĩnh viễn tài khoản thành công!");
                    }
                    catch
                    {
                        return (false, "Không thể xóa cứng do có dữ liệu ràng buộc. Hãy dùng xóa mềm!");
                    }
                }
                else if (new[] { "deactivate", "inactive", "lock" }.Contains(safeAction))
                    user.Status = "Inactive";
                else if (new[] { "activate", "active", "unlock" }.Contains(safeAction))
                    user.Status = "Active";
                else
                    return (false, $"Hành động '{action}' không hợp lệ!");
            }
            else
            {
                return (false, "Bạn không có quyền thực hiện chức năng này!");
            }

            await _userRepository.UpdateUserAsync(user);

            string actionDesc = user.Status == "Inactive" ? "bị khóa" : "được mở khóa";
            string authority = safeRole == AppRoles.Admin ? "Admin" : "Quản lý chi nhánh";

            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: user.ShowroomId,
                roleTarget: AppRoles.Manager,
                title: "Biến động nhân sự ⚠️",
                content: $"Tài khoản {user.FullName} ({user.Role}) vừa {actionDesc} bởi {authority}.",
                actionUrl: "/admin/users",
                type: "System"
            );

            return (true, "Đã cập nhật trạng thái nhân viên thành công!");
        }

        public async Task<(bool Success, string Message)> UpdateStaffAccountAsync(
            int targetUserId, UserUpdateRequestDto request,
            string currentUserRole, int? currentUserShowroomId)
        {
            var user = await _userRepository.GetUserByIdAsync(targetUserId);
            if (user == null || user.IsDeleted)
                return (false, "Không tìm thấy người dùng hoặc tài khoản đã bị xóa!");

            var allValidRoles = new List<string>(_staffRoles) { AppRoles.Manager, AppRoles.Admin };

            if (!allValidRoles.Contains(request.Role))
                return (false, $"Role '{request.Role}' không hợp lệ trong hệ thống!");

            var showroom = await _showroomRepository.GetByIdAsync(request.ShowroomId);
            if (showroom == null)
                return (false, "Showroom không tồn tại!");

            string safeRole = currentUserRole?.Trim() ?? "";

            if (safeRole == AppRoles.Manager)
            {
                if (user.Role == AppRoles.Admin || user.Role == AppRoles.Manager)
                    return (false, "Quản lý không có quyền chỉnh sửa cấp trên hoặc đồng cấp!");

                if (!currentUserShowroomId.HasValue || user.ShowroomId != currentUserShowroomId.Value)
                    return (false, "Nhân viên này thuộc chi nhánh khác!");

                if (!_staffRoles.Contains(request.Role))
                    return (false, "Quản lý chỉ được gán chức vụ nhân viên cấp dưới!");

                if (request.ShowroomId != currentUserShowroomId.Value)
                    return (false, "Không được chuyển nhân viên sang chi nhánh khác!");
            }
            else if (safeRole != AppRoles.Admin)
            {
                return (false, "Bạn không có quyền thực hiện chức năng này!");
            }

            user.FullName = request.FullName;
            user.Email = request.Email;
            user.Phone = request.Phone;
            user.Role = request.Role;
            user.ShowroomId = request.ShowroomId;

            var status = request.Status?.Trim();
            if (!string.IsNullOrEmpty(status))
            {
                if (status != "Active" && status != "Inactive")
                    return (false, "Status chỉ nhận Active hoặc Inactive!");
                user.Status = status;
            }

            await _userRepository.UpdateUserAsync(user);
            return (true, $"Đã cập nhật thông tin {user.FullName} thành công!");
        }
    }
}