using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
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

        // LẤY DANH SÁCH (Có bộ lọc phân khu và quyền)
        public async Task<object> GetFilteredUsersAsync(
            string userType, bool isDeleted, string? search, int page, int pageSize, 
            string currentUserRole, int? currentUserShowroomId, 
            int? filterShowroomId = null) // 👈 THÊM CÁI NÀY NÈ
        {
            // 👇 CHUYỀN TRÁI BÓNG XUỐNG REPO (Thêm filterShowroomId vào cuối)
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
                AvatarUrl = u.AvatarUrl,
                CreatedAt = u.CreatedAt,
                DeletedAt = u.DeletedAt
            });

            return new
            {
                Data = userDtos,
                TotalCount = result.TotalCount
            };
        }

        // TẠO TÀI KHOẢN NHÂN VIÊN (Bản có Phân quyền)
        public async Task<(bool Success, string Message)> CreateStaffAccountAsync(StaffAccountRequestDto request, string currentUserRole, int? currentUserShowroomId)
        {
            // 👇 BÍ KÍP PHÂN QUYỀN Ở ĐÂY 👇
            if (currentUserRole == "ShowroomManager")
            {
                // Quản lý không được phép tạo 1 Quản lý khác
                if (request.Role != "ShowroomSales")
                    return (false, "Sếp chỉ được phép tạo tài khoản cho Nhân viên (Sales) thôi ạ!");

                // Quản lý không được đưa nhân viên sang chi nhánh khác
                if (request.ShowroomId != currentUserShowroomId)
                    return (false, "Sếp chỉ được phép bổ nhiệm nhân viên vào Showroom của mình!");
            }
            else if (currentUserRole != "Admin")
            {
                return (false, "Bạn không có quyền thực hiện chức năng này!");
            }

            var validRoles = new List<string> { "ShowroomManager", "ShowroomSales" };
            if (!validRoles.Contains(request.Role))
                return (false, "Quyền (Role) không hợp lệ.");

            var existingUser = await _userRepository.GetUserByUsernameAsync(request.Username);
            if (existingUser != null)
                return (false, "Tên đăng nhập này đã có người sử dụng!");

            var showroom = await _showroomRepository.GetByIdAsync(request.ShowroomId);
            if (showroom == null)
                return (false, "Showroom không tồn tại trong hệ thống!");

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
            return (true, $"Tạo tài khoản {request.Role} cho {request.FullName} tại {showroom.Name} thành công!");
        }

        // KHÓA HOẶC XÓA TÀI KHOẢN (Chia quyền Admin/Manager)
        public async Task<(bool Success, string Message)> HandleUserStatusAsync(int targetUserId, string action, int currentUserId, string currentUserRole, int? currentUserShowroomId)
        {
            var user = await _userRepository.GetUserByIdAsync(targetUserId);
            if (user == null || user.IsDeleted) return (false, "Không tìm thấy người dùng hoặc đã bị xóa!");

            // Không ai được tự khóa/xóa chính mình
            if (user.UserId == currentUserId) return (false, "Đừng tự hủy ní ơi, không tự khóa tài khoản mình được đâu!");

            // 👇 QUẢN LÝ (MANAGER) RA TAY
            if (currentUserRole == "ShowroomManager")
            {
                if (user.Role == "Admin" || user.Role == "ShowroomManager")
                    return (false, "Sếp không có quyền thao tác lên cấp trên hoặc đồng cấp!");

                if (user.ShowroomId != currentUserShowroomId)
                    return (false, "Nhân viên này thuộc chi nhánh khác, sếp không quản lý được!");

                if (action == "Delete")
                    return (false, "Sếp chỉ được quyền Khóa (Vô hiệu hóa) nhân viên, Xóa là việc của Admin!");

                // Manager chỉ được Khóa (Inactive) / Mở khóa (Active)
                user.Status = action == "Deactivate" ? "Inactive" : "Active";
                //user.UpdatedAt = DateTime.Now;

                await _userRepository.UpdateUserAsync(user);
                return (true, $"Đã {(action == "Deactivate" ? "khóa" : "mở khóa")} nhân viên thành công!");
            }

            // 👇 ADMIN RA TAY (Đấng tối cao)
            if (currentUserRole == "Admin")
            {
                if (action == "Delete")
                {
                    user.IsDeleted = true; // Admin chém phát là Xóa mềm luôn
                    user.DeletedAt = DateTime.Now;
                    user.DeletedBy = currentUserId;
                    user.Status = "Inactive";
                    await _userRepository.UpdateUserAsync(user);
                    return (true, "Đã xóa tài khoản ra khỏi hệ thống!");
                }
                else
                {
                    user.Status = action == "Deactivate" ? "Inactive" : "Active";
                    //user.UpdatedAt = DateTime.Now;
                    await _userRepository.UpdateUserAsync(user);
                    return (true, $"Đã {(action == "Deactivate" ? "khóa" : "mở khóa")} tài khoản thành công!");
                }
            }

            return (false, "Lỗi quyền truy cập!");
        }
    }
}
