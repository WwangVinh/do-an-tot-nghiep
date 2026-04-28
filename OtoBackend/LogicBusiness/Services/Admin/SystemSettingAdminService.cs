using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Interfaces.Shared; // 👈 Thêm thư viện này để xài INotificationService
using LogicBusiness.DTOs; // 👈 Thêm để lấy AppRoles
using System.Threading.Tasks;

namespace LogicBusiness.Services.Admin
{
    public class SystemSettingAdminService : ISystemSettingAdminService
    {
        private readonly ISystemSettingRepository _repo;
        private readonly INotificationService _notiService; // 👈 Bơm súng vào đây

        public SystemSettingAdminService(ISystemSettingRepository repo, INotificationService notiService)
        {
            _repo = repo;
            _notiService = notiService;
        }

        public async Task<string?> GetSettingValueAsync(string key)
        {
            var setting = await _repo.GetByKeyAsync(key);
            return setting?.SettingValue;
        }

        public async Task<(bool Success, string Message)> UpdateSettingAsync(string key, string value)
        {
            var existing = await _repo.GetByKeyAsync(key);
            if (existing == null)
            {
                return (false, "Không tìm thấy cấu hình này trong hệ thống!");
            }

            // Lưu lại giá trị cũ để làm câu thông báo cho ngầu
            string oldValue = existing.SettingValue;

            // Cập nhật giá trị mới
            existing.SettingValue = value.Trim();
            await _repo.UpdateAsync(existing);

            // 👇 BẮN THÔNG BÁO CHO TRÙM CUỐI
            // Chỉ báo khi thực sự có sự thay đổi giá trị
            if (oldValue != existing.SettingValue)
            {
                await _notiService.CreateNotificationAsync(
                    userId: null,
                    showroomId: null, // Setting là của chung toàn server
                    roleTarget: AppRoles.Admin, // 👈 Trùm cuối Admin mới được nhận tin này
                    title: "⚙️ Cập nhật cấu hình hệ thống",
                    content: $"Cấu hình '{key}' vừa được thay đổi từ '{oldValue}' sang '{existing.SettingValue}'.",
                    actionUrl: "/admin/settings", // Trỏ về trang cài đặt hệ thống
                    type: "SystemAlert" // Báo động hệ thống
                );
            }

            return (true, "Cập nhật cấu hình thành công!");
        }
    }
}