using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Interfaces.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicBusiness.Services.Admin
{
    public class PromotionAdminService : IPromotionAdminService
    {
        private readonly IPromotionRepository _repo;
        private readonly INotificationService _notiService;
        public PromotionAdminService(IPromotionRepository repo, INotificationService notiService) { _repo = repo; _notiService = notiService; }

        public async Task<IEnumerable<PromotionAdminDto>> GetAllPromotionsAsync()
        {
            var data = await _repo.GetAllAsync();
            return data.Select(p => new PromotionAdminDto
            {
                PromotionId = p.PromotionId,
                Code = p.Code,
                Description = p.Description,
                DiscountPercentage = p.DiscountPercentage,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status,
                CarId = p.CarId,
                CarName = p.Car?.Name,
                MaxUsage = p.MaxUsage,
                CurrentUsage = p.CurrentUsage
            });
        }

        public async Task<(bool Success, string Message)> CreatePromotionAsync(PromotionCreateUpdateDto dto)
        {
            if (await _repo.CheckCodeExistsAsync(dto.Code)) return (false, "Mã giảm giá này đã tồn tại!");
            if (dto.EndDate.HasValue && dto.StartDate.HasValue && dto.EndDate < dto.StartDate) return (false, "Ngày kết thúc không hợp lệ!");

            var promotion = new Promotion
            {
                Code = dto.Code.Trim().ToUpper(),
                Description = dto.Description,
                DiscountPercentage = dto.DiscountPercentage,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                CarId = dto.CarId == 0 ? null : dto.CarId,
                MaxUsage = dto.MaxUsage,
                CurrentUsage = 0
            };

            await _repo.AddAsync(promotion);

            // 👇 BẮN THÔNG BÁO CHO NỘI BỘ
            if (promotion.Status == "Active") // Nếu mã kích hoạt luôn thì báo
            {
                await _notiService.CreateNotificationAsync(
                    userId: null,
                    showroomId: null,
                    roleTarget: $"{AppRoles.Marketing},{AppRoles.Sales},{AppRoles.ShowroomSales}", // Gọi Marketing và Sales
                    title: "Tung mã khuyến mãi mới! 🎁",
                    content: $"Mã {promotion.Code} (giảm {promotion.DiscountPercentage}%) vừa được kích hoạt. Marketing lên Banner, Sales lấy mã chốt khách ngay!",
                    actionUrl: "/admin/promotions",
                    type: "Promotion"
                );
            }

            return (true, "Tạo mã giảm giá thành công!");
        }

        public async Task<(bool Success, string Message)> UpdatePromotionAsync(int id, PromotionCreateUpdateDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return (false, "Không tìm thấy mã giảm giá!");
            if (await _repo.CheckCodeExistsAsync(dto.Code, id)) return (false, "Mã giảm giá này bị trùng với mã khác!");

            existing.Code = dto.Code.Trim().ToUpper();
            existing.Description = dto.Description;
            existing.DiscountPercentage = dto.DiscountPercentage;
            existing.StartDate = dto.StartDate;
            existing.EndDate = dto.EndDate;
            existing.Status = dto.Status;
            existing.CarId = dto.CarId == 0 ? null : dto.CarId;
            existing.MaxUsage = dto.MaxUsage;

            await _repo.UpdateAsync(existing);
            return (true, "Cập nhật thành công!");
        }

        public async Task<(bool Success, string Message)> DeletePromotionAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return (false, "Không tìm thấy mã giảm giá!");

            if (existing.CurrentUsage > 0) return (false, "Mã này đã có khách sử dụng, không thể xóa!");

            await _repo.DeleteAsync(existing);
            return (true, "Xóa mã giảm giá thành công!");
        }
    }
}