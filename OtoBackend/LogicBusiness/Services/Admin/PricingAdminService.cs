using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Interfaces.Shared;

namespace LogicBusiness.Services.Admin
{
    public class PricingAdminService : IPricingAdminService
    {
        private readonly ICarPricingVersionRepository _pricingRepository;
        private readonly INotificationService _notiService;

        public PricingAdminService(ICarPricingVersionRepository pricingRepository, INotificationService notiService)
        {
            _pricingRepository = pricingRepository;
            _notiService = notiService;
        }

        public async Task<IEnumerable<PricingVersionAdminDto>> GetAllAsync(int? carId = null, bool? isActive = null)
        {
            var items = await _pricingRepository.GetAllAsync(carId, isActive);
            return items.Select(MapAdminDto);
        }

        public async Task<(bool IsSuccess, string Message, PricingVersionAdminDto? Data)> CreateAsync(PricingVersionUpsertDto dto)
        {
            if (!await _pricingRepository.CarExistsAsync(dto.CarId))
            {
                return (false, "Xe không tồn tại hoặc đã bị xóa.", null);
            }

            if (string.IsNullOrWhiteSpace(dto.VersionName))
            {
                return (false, "Tên phiên bản không được để trống.", null);
            }

            var entity = new CarPricingVersion
            {
                CarId = dto.CarId,
                VersionName = dto.VersionName.Trim(),
                PriceVnd = dto.PriceVnd,
                SortOrder = dto.SortOrder,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _pricingRepository.AddAsync(entity);
            var created = await _pricingRepository.GetByIdAsync(entity.PricingVersionId);

            string carNameDisplay = created?.Car?.Name ?? $"ID {dto.CarId}";

            // Bắn thông báo cho toàn hệ thống
            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: null, // Giá áp dụng chung cho mọi Showroom
                roleTarget: $"{AppRoles.Admin},{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales},{AppRoles.Marketing}",
                title: "Bảng giá mới cập nhật 💰",
                content: $"Mẫu xe {carNameDisplay} vừa có thêm phiên bản: {dto.VersionName} (Giá: {dto.PriceVnd:N0}đ).",
                actionUrl: $"/admin/cars/detail/{dto.CarId}", // Trỏ về trang chi tiết xe
                type: "Pricing"
            );

            return (true, "Tạo phiên bản giá thành công.", created == null ? null : MapAdminDto(created));
        }

        public async Task<(bool IsSuccess, string Message, PricingVersionAdminDto? Data)> UpdateAsync(int id, PricingVersionUpsertDto dto)
        {
            var entity = await _pricingRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return (false, "Không tìm thấy phiên bản giá.", null);
            }

            if (!await _pricingRepository.CarExistsAsync(dto.CarId))
            {
                return (false, "Xe không tồn tại hoặc đã bị xóa.", null);
            }

            if (string.IsNullOrWhiteSpace(dto.VersionName))
            {
                return (false, "Tên phiên bản không được để trống.", null);
            }

            entity.CarId = dto.CarId;
            entity.VersionName = dto.VersionName.Trim();
            entity.PriceVnd = dto.PriceVnd;
            entity.SortOrder = dto.SortOrder;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.Now;

            await _pricingRepository.UpdateAsync(entity);
            var updated = await _pricingRepository.GetByIdAsync(id);

            string carNameDisplay = updated?.Car?.Name ?? $"ID {dto.CarId}";

            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: null,
                roleTarget: $"{AppRoles.Admin},{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales},{AppRoles.Marketing}",
                title: "🚨 CẢNH BÁO: Thay đổi giá xe!",
                content: $"Phiên bản {dto.VersionName} của mẫu {carNameDisplay} vừa được cập nhật giá mới thành {dto.PriceVnd:N0}đ. Anh em Sale lưu ý báo giá khách!",
                actionUrl: $"/admin/cars/detail/{dto.CarId}",
                type: "PricingAlert" // Đặt type khác đi xíu để FE hiện màu đỏ
            );

            return (true, "Cập nhật phiên bản giá thành công.", updated == null ? null : MapAdminDto(updated));
        }

        public async Task<(bool IsSuccess, string Message)> DeleteAsync(int id)
        {
            var entity = await _pricingRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return (false, "Không tìm thấy phiên bản giá.");
            }

            // 👈 Lưu tạm thông tin trước khi bóp cò xóa
            string carNameDisplay = entity.Car?.Name ?? $"ID {entity.CarId}";
            string versionName = entity.VersionName;
            int carId = entity.CarId;

            // Xóa khỏi Database
            await _pricingRepository.DeleteAsync(entity);

            // 👈 Bắn thông báo
            await _notiService.CreateNotificationAsync(
                userId: null,
                showroomId: null,
                roleTarget: $"{AppRoles.Admin},{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales}", // Không cần báo Marketing vụ này
                title: "Xóa phiên bản xe 🗑️",
                content: $"Phiên bản {versionName} của mẫu {carNameDisplay} đã bị gỡ khỏi hệ thống báo giá.",
                actionUrl: $"/admin/cars/detail/{carId}",
                type: "Pricing"
            );

            return (true, "Đã xóa phiên bản giá.");
        }

        private static PricingVersionAdminDto MapAdminDto(CarPricingVersion entity)
        {
            return new PricingVersionAdminDto
            {
                PricingVersionId = entity.PricingVersionId,
                CarId = entity.CarId,
                CarName = entity.Car?.Name ?? string.Empty,
                Brand = entity.Car?.Brand,
                VersionName = entity.VersionName,
                PriceVnd = entity.PriceVnd,
                SortOrder = entity.SortOrder,
                IsActive = entity.IsActive
            };
        }
    }
}
