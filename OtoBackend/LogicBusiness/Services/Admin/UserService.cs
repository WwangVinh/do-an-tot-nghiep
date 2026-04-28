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

        private readonly List<string> _staffRoles = new List<string>
        {
            "Sales", "Technician", "Content", "Marketing", "ShowroomSales"
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

        // 1. TẠO TÀI KHOẢN (Logic phân cấp Manager vs Admin)
        public async Task<(bool Success, string Message)> CreateStaffAccountAsync(StaffAccountRequestDto request, string currentUserRole, int? currentUserShowroomId)
        {
            string safeRole = currentUserRole?.Trim();

            // 👇 LOGIC PHÂN QUYỀN "AO NHÀ" 👇
            if (safeRole == "ShowroomManager" || safeRole == "SalesManager")
            {
                // Manager chỉ được tạo nhân viên cấp dưới (Sale, Kỹ thuật, Content, Marketing)
                if (!_staffRoles.Contains(request.Role))
                    return (false, "Sếp chỉ được phép tạo tài khoản cho nhân viên cấp dưới (Sale/Kỹ thuật/Content/Marketing)!");

                // Manager không được "lấn sân" sang showroom khác
                if (request.ShowroomId != currentUserShowroomId)
                    return (false, "Sếp chỉ được phép bổ nhiệm nhân viên vào Showroom mình đang quản lý!");
            }
            else if (safeRole != "Admin")
            {
                return (false, "Ní không có thẻ ngành (quyền) để thực hiện chức năng này!");
            }

            // Kiểm tra Role có nằm trong danh mục hệ thống cho phép không
            var allValidRoles = new List<string>(_staffRoles) { "ShowroomManager", "SalesManager", "Admin" };
            if (!allValidRoles.Contains(request.Role))
                return (false, "Chức vụ (Role) này hệ thống chưa hỗ trợ ní ơi!");

            var existingUser = await _userRepository.GetUserByUsernameAsync(request.Username);
            if (existingUser != null)
                return (false, "Tên đăng nhập này đã có người sử dụng!");

            var showroom = await _showroomRepository.GetByIdAsync(request.ShowroomId);
            if (showroom == null)
                return (false, "Showroom này không tồn tại trên bản đồ hệ thống!");

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
                content: $"Chào mừng {request.FullName} ({request.Role}) vừa gia nhập đại gia đình chi nhánh chúng ta.",
                actionUrl: "/admin/users",
                type: "System"
            );

            return (true, $"Tạo tài khoản {request.Role} thành công cho {request.FullName}!");
        }

        public async Task<(bool Success, string Message)> HandleUserStatusAsync(int targetUserId, string action, int currentUserId, string currentUserRole, int? currentUserShowroomId)
        {
            var user = await _userRepository.GetUserByIdAsync(targetUserId);
            if (user == null || user.IsDeleted)
                return (false, "Không tìm thấy người dùng hoặc tài khoản đã bị xóa!");

            // Chặn tự hủy: Không ai được tự khóa/xóa chính mình
            if (user.UserId == currentUserId)
                return (false, "Đừng tự hủy ní ơi, không tự khóa tài khoản mình được đâu!");

            string safeAction = action?.Trim().ToLower() ?? "";
            string safeRole = currentUserRole?.Trim().ToLower() ?? "";

            // ---------------------------------------------------------
            // 1. LOGIC CHO QUẢN LÝ (ShowroomManager / SalesManager)
            // ---------------------------------------------------------
            if (safeRole == "showroommanager" || safeRole == "salesmanager")
            {
                // Kiểm tra quyền hạn: Manager không được đụng vào Admin hoặc các Manager khác
                if (user.Role == "Admin" || user.Role == "ShowroomManager" || user.Role == "SalesManager")
                    return (false, "Sếp không có quyền thao tác lên cấp trên hoặc đồng cấp!");

                // Kiểm tra phạm vi: Chỉ được quản lý nhân viên trong đúng Showroom của mình
                if (!currentUserShowroomId.HasValue || user.ShowroomId != currentUserShowroomId.Value)
                    return (false, "Nhân viên này thuộc chi nhánh khác, sếp không quản lý được!");

                // Giới hạn hành động: Manager chỉ được Khóa/Mở, không được Xóa (Xóa là việc của Admin)
                if (safeAction == "delete" || safeAction == "hard_delete")
                    return (false, "Sếp chỉ có quyền Khóa/Mở khóa nhân viên. Việc xóa tài khoản hãy để Admin ra tay!");

                // Thực hiện thay đổi trạng thái
                if (new[] { "deactivate", "inactive", "lock" }.Contains(safeAction))
                    user.Status = "Inactive";
                else if (new[] { "activate", "active", "unlock" }.Contains(safeAction))
                    user.Status = "Active";
                else
                    return (false, $"Hành động '{action}' không hợp lệ cho cấp Quản lý!");
            }

            // ---------------------------------------------------------
            // 2. LOGIC CHO ADMIN (Đấng tối cao)
            // ---------------------------------------------------------
            else if (safeRole == "admin")
            {
                // XÓA MỀM (Soft Delete)
                if (safeAction == "delete")
                {
                    user.IsDeleted = true;
                    user.DeletedAt = DateTime.Now;
                    user.DeletedBy = currentUserId;
                    user.Status = "Inactive";
                }
                // XÓA CỨNG (Hard Delete) - Bứng gốc vĩnh viễn
                else if (safeAction == "hard_delete")
                {
                    try
                    {
                        string deletedName = user.FullName;
                        int? showroomId = user.ShowroomId;

                        await _userRepository.HardDeleteUserAsync(user);

                        // Nếu có showroom, bắn tin cho sếp chi nhánh biết
                        if (showroomId.HasValue)
                        {
                            await _notiService.CreateNotificationAsync(
                                userId: null,
                                showroomId: showroomId.Value,
                                roleTarget: "ShowroomManager", // 👈 Đã sửa: Chỉ báo cho Quản lý chi nhánh
                                title: "Tài khoản bị XÓA VĨNH VIỄN ☠️",
                                content: $"Admin vừa gạch tên vĩnh viễn {deletedName} khỏi hệ thống chi nhánh.",
                                actionUrl: "/admin/users",
                                type: "System"
                            );
                        }
                        return (true, "Đã xóa vĩnh viễn tài khoản thành công!");
                    }
                    catch (Exception)
                    {
                        return (false, "Không thể xóa cứng do người này có dữ liệu ràng buộc (đơn hàng/lịch hẹn). Hãy dùng Xóa mềm!");
                    }
                }
                // KHÓA / MỞ KHÓA
                else if (new[] { "deactivate", "inactive", "lock" }.Contains(safeAction))
                    user.Status = "Inactive";
                else if (new[] { "activate", "active", "unlock" }.Contains(safeAction))
                    user.Status = "Active";
                else
                    return (false, $"Hành động '{action}' không hợp lệ!");
            }
            else
            {
                return (false, "Lỗi quyền truy cập! Chức vụ của bạn không có quyền thực hiện việc này.");
            }

            // ---------------------------------------------------------
            // 3. HOÀN TẤT: LƯU VÀ BẮN THÔNG BÁO
            // ---------------------------------------------------------
            await _userRepository.UpdateUserAsync(user);

            string actionDesc = user.Status == "Inactive" ? "bị khóa/đình chỉ" : "được khôi phục hoạt động";
            string authority = safeRole == "admin" ? "Admin hệ thống" : "Quản lý chi nhánh";

            // 👈 Đã sửa: Bắn thông báo cho Sếp của chi nhánh thay vì bắn cho người bị khóa
            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: user.ShowroomId,
                roleTarget: "ShowroomManager",
                title: "Biến động nhân sự ⚠️",
                content: $"Tài khoản của nhân viên {user.FullName} ({user.Role}) vừa {actionDesc} bởi {authority}.",
                actionUrl: "/admin/users",
                type: "System"
            );

            return (true, $"Đã cập nhật trạng thái cho nhân viên thành công!");
        }

        public async Task<(bool Success, string Message)> UpdateStaffAccountAsync(
             int targetUserId,
             UserUpdateRequestDto request,
             string currentUserRole,
             int? currentUserShowroomId)
        {
            var user = await _userRepository.GetUserByIdAsync(targetUserId);
            if (user == null || user.IsDeleted)
                return (false, "Không tìm thấy người dùng hoặc tài khoản đã bị xóa!");

            // 1. Định nghĩa các nhóm quyền để check cho gọn
            var staffRoles = new List<string> { "Sales", "Technician", "Content", "Marketing", "ShowroomSales" };
            var managerRoles = new List<string> { "ShowroomManager", "SalesManager" };
            var allValidRoles = staffRoles.Concat(managerRoles).Concat(new[] { "Admin" }).ToList();

            // Kiểm tra Role mới có nằm trong hệ thống không
            if (!allValidRoles.Contains(request.Role))
                return (false, "Chức vụ (Role) mới không hợp lệ trong hệ thống!");

            var showroom = await _showroomRepository.GetByIdAsync(request.ShowroomId);
            if (showroom == null)
                return (false, "Showroom này không tồn tại!");

            string safeRole = currentUserRole?.Trim().ToLower() ?? "";

            // ---------------------------------------------------------
            // 2. PHÂN QUYỀN CHỈNH SỬA
            // ---------------------------------------------------------
            if (safeRole == "showroommanager" || safeRole == "salesmanager")
            {
                // Manager không được sửa Admin hoặc các Manager khác
                if (user.Role == "Admin" || managerRoles.Contains(user.Role))
                    return (false, "Sếp không có quyền chỉnh sửa cấp trên hoặc đồng cấp!");

                // Chỉ được sửa nhân viên trong "ao nhà" mình
                if (!currentUserShowroomId.HasValue || user.ShowroomId != currentUserShowroomId.Value)
                    return (false, "Nhân viên này thuộc chi nhánh khác, sếp không quản lý được!");

                // Manager chỉ được gán các quyền nhân viên (không được tự ý bổ nhiệm Manager mới)
                if (!staffRoles.Contains(request.Role))
                    return (false, "Sếp chỉ được phép gán các chức vụ nhân viên cấp dưới!");

                // Không được đẩy nhân viên sang showroom khác
                if (request.ShowroomId != currentUserShowroomId.Value)
                    return (false, "Sếp không được phép chuyển nhân viên sang chi nhánh khác!");
            }
            else if (safeRole != "admin")
            {
                return (false, "Ní không có quyền thực hiện chức năng này!");
            }

            // ---------------------------------------------------------
            // 3. CẬP NHẬT DỮ LIỆU
            // ---------------------------------------------------------
            user.FullName = request.FullName;
            user.Email = request.Email;
            user.Phone = request.Phone;
            user.Role = request.Role;
            user.ShowroomId = request.ShowroomId;

            var status = request.Status?.Trim();
            if (!string.IsNullOrEmpty(status))
            {
                if (status != "Active" && status != "Inactive")
                    return (false, "Trạng thái (Status) chỉ nhận Active hoặc Inactive!");
                user.Status = status;
            }

            await _userRepository.UpdateUserAsync(user);
            return (true, $"Đã cập nhật thông tin cho {user.FullName} thành công!");
        }
    }
}
