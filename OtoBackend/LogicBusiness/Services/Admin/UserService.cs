using CoreEntities.Models;
using CoreEntities.Models.DTOs;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Interfaces.Shared;
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
        private readonly INotificationService _notiService;

        public UserService(IUserRepository userRepository, IShowroomRepository showroomRepository, INotificationService notiService)
        {
            _userRepository = userRepository;
            _showroomRepository = showroomRepository;
            _notiService = notiService;
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
                ShowroomId = u.ShowroomId,
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
            await _notiService.CreateNotificationAsync(
                userId: null,
                roleTarget: null,
                showroomId: request.ShowroomId, // Báo cho cả lò chi nhánh đó biết
                title: "Nhân sự mới gia nhập! 🎉",
                content: $"Chào mừng {request.FullName} vừa được cấp tài khoản {request.Role} tại chi nhánh chúng ta.",
                actionUrl: "/admin/users", // Trỏ về trang quản lý nhân sự
                type: "System"
            );
            return (true, $"Tạo tài khoản {request.Role} cho {request.FullName} tại {showroom.Name} thành công!");
        }

        // KHÓA HOẶC XÓA TÀI KHOẢN (Bản nâng cấp chống lỗi hoa/thường)
        public async Task<(bool Success, string Message)> HandleUserStatusAsync(int targetUserId, string action, int currentUserId, string currentUserRole, int? currentUserShowroomId)
        {
            var user = await _userRepository.GetUserByIdAsync(targetUserId);
            if (user == null || user.IsDeleted) return (false, "Không tìm thấy người dùng hoặc đã bị xóa!");

            // Không ai được tự khóa/xóa chính mình
            if (user.UserId == currentUserId) return (false, "Đừng tự hủy ní ơi, không tự khóa tài khoản mình được đâu!");

            // 👇 1. CHUẨN HÓA CHUỖI: Bắt Frontend gửi gì cũng chuyển về chữ thường hết để dễ so sánh
            string safeAction = action?.Trim().ToLower() ?? "";
            string safeRole = currentUserRole?.Trim().ToLower() ?? "";

            // 👇 QUẢN LÝ (MANAGER) RA TAY
            if (safeRole == "showroommanager")
            {
                if (user.Role == "Admin" || user.Role == "ShowroomManager")
                    return (false, "Sếp không có quyền thao tác lên cấp trên hoặc đồng cấp!");

                if (user.ShowroomId != currentUserShowroomId)
                    return (false, "Nhân viên này thuộc chi nhánh khác, sếp không quản lý được!");

                if (safeAction == "delete")
                    return (false, "Sếp chỉ được quyền Khóa (Vô hiệu hóa) nhân viên, Xóa là việc của Admin!");

                // Manager chỉ được Khóa / Mở khóa
                if (safeAction == "deactivate" || safeAction == "inactive" || safeAction == "lock")
                {
                    user.Status = "Inactive";
                }
                else if (safeAction == "activate" || safeAction == "active" || safeAction == "unlock")
                {
                    user.Status = "Active";
                }
                else
                {
                    return (false, $"Hành động '{action}' từ hệ thống không hợp lệ!");
                }

                await _userRepository.UpdateUserAsync(user);

                string actionText = user.Status == "Inactive" ? "bị đình chỉ hoạt động" : "được mở khóa lại";
                await _notiService.CreateNotificationAsync(
                    userId: targetUserId,
                    showroomId: null,
                    roleTarget: null,
                    title: "Thay đổi trạng thái tài khoản ⚠️",
                    content: $"Tài khoản của bạn vừa {actionText} bởi Quản lý. Liên hệ sếp nếu có thắc mắc.",
                    actionUrl: "#",
                    type: "System"
                );
                return (true, $"Đã {(user.Status == "Inactive" ? "khóa" : "mở khóa")} nhân viên thành công!");
            }

            // 👇 ADMIN RA TAY (Đấng tối cao)
            else if (safeRole == "admin")
            {
                if (safeAction == "delete")
                {
                    user.IsDeleted = true; // Admin chém phát là Xóa mềm luôn
                    user.DeletedAt = DateTime.Now;
                    user.DeletedBy = currentUserId;
                    user.Status = "Inactive";
                    await _userRepository.UpdateUserAsync(user);

                    if (user.ShowroomId.HasValue)
                    {
                        await _notiService.CreateNotificationAsync(
                            userId: null,
                            roleTarget: null,
                            showroomId: user.ShowroomId.Value,
                            title: "Tài khoản bị Xóa ❌",
                            content: $"Admin vừa gạch tên {user.FullName} ({user.Role}) khỏi hệ thống chi nhánh này.",
                            actionUrl: "/admin/users",
                            type: "System"
                        );
                    }
                    return (true, "Đã xóa tài khoản ra khỏi hệ thống!");
                }
                // 👇 THÊM NHÁNH XÓA CỨNG VÀO ĐÂY 👇
                else if (safeAction == "hard_delete")
                {
                    try
                    {
                        // 1. Lưu lại thông tin trước khi bứng gốc để tí còn biết tên mà báo cáo
                        string deletedName = user.FullName;
                        string deletedRole = user.Role;
                        int? showroomId = user.ShowroomId;

                        // 2. Ra đòn kết liễu
                        await _userRepository.HardDeleteUserAsync(user);

                        // 3. Bắn chuông cho Showroom
                        if (showroomId.HasValue)
                        {
                            await _notiService.CreateNotificationAsync(
                                userId: null,
                                roleTarget: null,
                                showroomId: showroomId.Value,
                                title: "Tài khoản bị XÓA VĨNH VIỄN ☠️",
                                content: $"Admin vừa bứng gốc tài khoản {deletedName} ({deletedRole}) khỏi hệ thống. Dữ liệu này không thể khôi phục!",
                                actionUrl: "/admin/users",
                                type: "System"
                            );
                        }

                        return (true, "Đã xóa CỨNG (vĩnh viễn) tài khoản ra khỏi hệ thống!");
                    }
                    catch (Exception)
                    {
                        // Bắt lỗi rớt khóa ngoại của SQL Server
                        return (false, "Không thể xóa vĩnh viễn người này vì họ đã có lịch sử giao dịch/đặt lịch trong hệ thống. Chỉ nên dùng Xóa mềm!");
                    }
                }
                // 👆 KẾT THÚC NHÁNH XÓA CỨNG 👆
                else
                {
                    // Admin Khóa / Mở khóa
                    if (safeAction == "deactivate" || safeAction == "inactive" || safeAction == "lock")
                    {
                        user.Status = "Inactive";
                    }
                    else if (safeAction == "activate" || safeAction == "active" || safeAction == "unlock")
                    {
                        user.Status = "Active";
                    }
                    else
                    {
                        return (false, $"Hành động '{action}' không hợp lệ!");
                    }

                    await _userRepository.UpdateUserAsync(user);

                    string actionText = user.Status == "Inactive" ? "bị khóa" : "được khôi phục hoạt động";
                    await _notiService.CreateNotificationAsync(
                        userId: targetUserId,
                        showroomId: null,
                        roleTarget: null,
                        title: "Thay đổi trạng thái tài khoản ⚠️",
                        content: $"Tài khoản của bạn vừa {actionText} bởi Admin hệ thống.",
                        actionUrl: "#",
                        type: "System"
                    );
                    return (true, $"Đã {(user.Status == "Inactive" ? "khóa" : "mở khóa")} tài khoản thành công!");
                }
            }

            return (false, "Lỗi quyền truy cập! Không xác định được chức vụ của bạn.");
        }
    }
}
