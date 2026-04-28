using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Interfaces.Shared;
using LogicBusiness.Utilities;
using System.Reflection.Metadata;

namespace LogicBusiness.Services.Admin
{
    public class BannerAdminService : IBannerAdminService
    {
        private readonly IBannerRepository _repo;
        private readonly INotificationService _notificationService;

        public BannerAdminService(IBannerRepository repo, INotificationService notificationService)
        {
            _repo = repo;
            _notificationService = notificationService;
        }

        public Task<List<Banner>> GetAllAsync(bool? isActive = null)
        {
            return _repo.GetAllAsync(isActive);
        }

        public Task<Banner?> GetByIdAsync(int id)
        {
            return _repo.GetByIdAsync(id);
        }

        public async Task<(bool Success, string Message, Banner? Data)> CreateAsync(BannerCreateDto dto, int currentUserId)
        {
            if (dto.EndDate.HasValue && dto.StartDate.HasValue && dto.EndDate.Value < dto.StartDate.Value)
            {
                return (false, "EndDate không được nhỏ hơn StartDate.", null);
            }

            var banner = new Banner
            {
                BannerName = dto.BannerName.Trim(),
                ImageUrl = dto.ImageUrl.Trim(),
                LinkUrl = string.IsNullOrWhiteSpace(dto.LinkUrl) ? null : dto.LinkUrl.Trim(),
                Position = dto.Position,
                IsActive = dto.IsActive,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            await _repo.AddAsync(banner);

            // Bắn thông báo: Người làm là currentUserId, người nhận là các Role khác
            await _notificationService.CreateNotificationAsync(
                currentUserId, // Truyền ID người làm vào đây
                null,
                $"{AppRoles.Admin},{AppRoles.Sales},{AppRoles.ShowroomSales}", // Bắn cho đội cần biết
                "🖼️ Banner mới đã lên sóng",
                $"Banner \"{banner.BannerName}\" vừa được tạo bởi {currentUserId}.", // Có thể query thêm tên người dùng nếu cần
                "/",
                "BANNER"
            );

            return (true, "Tạo banner thành công!", banner);
        }

        public async Task<(bool Success, string Message, Banner? Data)> UpdateAsync(int id, BannerUpdateDto dto, int currentUserId)
        {
            // 1. Validate ngày tháng
            if (dto.EndDate.HasValue && dto.StartDate.HasValue && dto.EndDate.Value < dto.StartDate.Value)
            {
                return (false, "EndDate không được nhỏ hơn StartDate.", null);
            }

            // 2. Kiểm tra banner có tồn tại không
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return (false, "Không tìm thấy banner!", null);

            // 3. XỬ LÝ ẢNH: Nếu FE có gửi link ảnh mới lên và link đó khác link cũ
            if (!string.IsNullOrWhiteSpace(dto.ImageUrl) && dto.ImageUrl.Trim() != existing.ImageUrl)
            {
                // Gọn gàng dọn dẹp cái ảnh cũ trên ổ cứng
                if (!string.IsNullOrWhiteSpace(existing.ImageUrl))
                {
                    try
                    {
                        FileHelper.DeleteFile(existing.ImageUrl);
                    }
                    catch
                    {
                        // Try-catch để lỡ file ảnh bị ai đó xóa tay trong thư mục rồi thì code không bị crash
                    }
                }
                // Gán link ảnh mới vào DB
                existing.ImageUrl = dto.ImageUrl.Trim();
            }

            // 4. Cập nhật các thông tin còn lại
            existing.BannerName = dto.BannerName.Trim();
            existing.LinkUrl = string.IsNullOrWhiteSpace(dto.LinkUrl) ? null : dto.LinkUrl.Trim();
            existing.Position = dto.Position;
            existing.IsActive = dto.IsActive;
            existing.StartDate = dto.StartDate;
            existing.EndDate = dto.EndDate;

            // 5. Lưu Database
            await _repo.UpdateAsync(existing);

            await _notificationService.CreateNotificationAsync(
                currentUserId, // Ép kiểu từ string sang int như ní vừa làm
                null,
                $"{AppRoles.Admin},{AppRoles.Marketing}", // Cập nhật thì chỉ cần Admin và Marketing biết để kiểm soát nội dung
                "🔄 Banner đã được cập nhật",
                $"Banner \"{existing.BannerName}\" vừa được thay đổi nội dung hoặc thời gian hiển thị.",
                "/admin/banners",
                "BANNER_UPDATE"
            );

            return (true, "Cập nhật banner thành công!", existing);
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id, int currentUserId)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
            {
                return (false, "Không tìm thấy banner để xóa!");
            }

            // ... (code xóa ảnh vật lý và xóa database của ní giữ nguyên) ...
            await _repo.DeleteAsync(existing);

            // Bắn thông báo có truyền currentUserId để biết ai xóa
            await _notificationService.CreateNotificationAsync(
                currentUserId, // 👈 Có biến này rồi thì hết lỗi
                null,
                $"{AppRoles.Admin},{AppRoles.Manager}",
                "⚠️ Banner đã bị gỡ bỏ",
                $"Banner ID {id} đã bị xóa khỏi hệ thống.",
                "/admin/banners",
                "BANNER"
            );

            return (true, "Xóa banner và file ảnh thành công!");
        }
    }
}

