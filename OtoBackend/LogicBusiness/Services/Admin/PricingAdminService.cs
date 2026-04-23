using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;

namespace LogicBusiness.Services.Admin
{
    public class PricingAdminService : IPricingAdminService
    {
        private readonly ICarPricingVersionRepository _pricingRepository;

        public PricingAdminService(ICarPricingVersionRepository pricingRepository)
        {
            _pricingRepository = pricingRepository;
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

            return (true, "Cập nhật phiên bản giá thành công.", updated == null ? null : MapAdminDto(updated));
        }

        public async Task<(bool IsSuccess, string Message)> DeleteAsync(int id)
        {
            var entity = await _pricingRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return (false, "Không tìm thấy phiên bản giá.");
            }

            await _pricingRepository.DeleteAsync(entity);
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
