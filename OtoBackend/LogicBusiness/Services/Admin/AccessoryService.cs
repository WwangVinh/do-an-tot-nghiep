using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Utilities;

namespace LogicBusiness.Services.Admin
{
    public class AccessoryService : IAccessoryService
    {
        private readonly IAccessoryRepository _repo;

        public AccessoryService(IAccessoryRepository repo)
        {
            _repo = repo;
        }

        // ── CRUD ─────────────────────────────────────────────────────────────────

        public async Task<IEnumerable<AccessoryResponseDto>> GetAllAsync()
        {
            var items = await _repo.GetAllAsync();
            return items.Select(ToDto);
        }

        public async Task<AccessoryResponseDto?> GetByIdAsync(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            return item == null ? null : ToDto(item);
        }

        public async Task<(bool Success, string Message, AccessoryResponseDto? Data)> CreateAsync(AccessoryCreateDto dto)
        {
            var accessory = new Accessory
            {
                Name = dto.Name.Trim(),
                Price = dto.Price,
                Description = dto.Description?.Trim(),
                IsActive = dto.IsActive,
            };

            if (dto.ImageFile != null)
                accessory.ImageUrl = await FileHelper.UploadFileAsync(dto.ImageFile, "Accessories", dto.Name.Trim());

            await _repo.AddAsync(accessory);
            return (true, "Thêm phụ kiện thành công!", ToDto(accessory));
        }

        public async Task<(bool Success, string Message)> UpdateAsync(int id, AccessoryUpdateDto dto)
        {
            var accessory = await _repo.GetByIdAsync(id);
            if (accessory == null) return (false, "Không tìm thấy phụ kiện!");

            accessory.Name = dto.Name.Trim();
            accessory.Price = dto.Price;
            accessory.Description = dto.Description?.Trim();
            accessory.IsActive = dto.IsActive;

            if (dto.ImageFile != null)
            {
                // Xóa ảnh cũ nếu có
                if (!string.IsNullOrEmpty(accessory.ImageUrl))
                    FileHelper.DeleteFile(accessory.ImageUrl);

                accessory.ImageUrl = await FileHelper.UploadFileAsync(dto.ImageFile, "Accessories", dto.Name.Trim());
            }

            await _repo.UpdateAsync(accessory);
            return (true, "Cập nhật phụ kiện thành công!");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var accessory = await _repo.GetByIdAsync(id);
            if (accessory == null) return (false, "Không tìm thấy phụ kiện!");

            // Xóa ảnh khỏi ổ cứng
            if (!string.IsNullOrEmpty(accessory.ImageUrl))
                FileHelper.DeleteFile(accessory.ImageUrl);

            await _repo.DeleteAsync(id);
            return (true, "Đã xóa phụ kiện!");
        }

        // ── CarAccessory ─────────────────────────────────────────────────────────

        public async Task<IEnumerable<AccessoryResponseDto>> GetByCarIdAsync(int carId)
        {
            var items = await _repo.GetByCarIdAsync(carId);
            return items.Select(ToDto);
        }

        public async Task<(bool Success, string Message)> AssignToCarAsync(int carId, AssignAccessoriesDto dto)
        {
            await _repo.AssignToCarAsync(carId, dto.AccessoryIds);
            return (true, $"Đã gán {dto.AccessoryIds.Count} phụ kiện cho xe!");
        }

        public async Task<(bool Success, string Message)> RemoveFromCarAsync(int carId, AssignAccessoriesDto dto)
        {
            await _repo.RemoveFromCarAsync(carId, dto.AccessoryIds);
            return (true, "Đã bỏ gán phụ kiện!");
        }

        // ── Helper ───────────────────────────────────────────────────────────────

        private static AccessoryResponseDto ToDto(Accessory a) => new()
        {
            AccessoryId = a.AccessoryId,
            Name = a.Name,
            Price = a.Price,
            Description = a.Description,
            ImageUrl = a.ImageUrl,
            IsActive = a.IsActive,
        };
    }
}