using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Services.Admin
{
    public class FeatureService : IFeatureService
    {
        private readonly IFeatureRepository _featureRepo;

        public FeatureService(IFeatureRepository featureRepo)
        {
            _featureRepo = featureRepo;
        }

        public async Task<IEnumerable<Feature>> GetAllFeaturesAsync(string? search = null)
        {
            return await _featureRepo.GetAllAsync(search);
        }

        public async Task<Feature> GetFeatureByIdAsync(int id)
        {
            return await _featureRepo.GetByIdAsync(id);
        }

        public async Task<(bool IsSuccess, string Message)> CreateFeatureAsync(FeatureCreateDto dto)
        {
            // 1. Chặn trùng tên ngay từ vòng gửi xe
            if (await _featureRepo.CheckExistsAsync(dto.FeatureName))
                return (false, $"Tính năng '{dto.FeatureName}' đã tồn tại trong hệ thống!");

            // 2. Up ảnh phẳng (Gọi hàm mới viết)
            string iconUrl = "/uploads/Features/default-feature.png";
            if (dto.Icon != null)
            {
                iconUrl = await FileHelper.UploadIconAsync(dto.Icon, "Features", dto.FeatureName);
            }

            var feature = new Feature { FeatureName = dto.FeatureName, Icon = iconUrl };
            await _featureRepo.AddAsync(feature);
            return (true, "Thêm tính năng thành công!");
        }

        public async Task<(bool IsSuccess, string Message)> UpdateFeatureAsync(int id, FeatureUpdateDto dto)
        {
            // 1. Chặn trùng tên với Feature khác
            if (await _featureRepo.CheckExistsAsync(dto.FeatureName, id))
                return (false, $"Tên '{dto.FeatureName}' đã bị trùng với tính năng khác!");

            var existingFeature = await _featureRepo.GetByIdAsync(id);
            if (existingFeature == null) return (false, "Không tìm thấy tính năng!");

            // 2. Xử lý ảnh
            if (dto.Icon != null)
            {
                // Xóa ảnh cũ (nếu không phải là ảnh mặc định)
                if (!string.IsNullOrEmpty(existingFeature.Icon) && !existingFeature.Icon.Contains("default-feature.png"))
                {
                    FileHelper.DeleteFile(existingFeature.Icon);
                }

                // Up ảnh mới và đè tên
                existingFeature.Icon = await FileHelper.UploadIconAsync(dto.Icon, "Features", dto.FeatureName);
            }

            existingFeature.FeatureName = dto.FeatureName;
            await _featureRepo.UpdateAsync(existingFeature);

            return (true, "Cập nhật thành công!");
        }

        public async Task<(bool IsSuccess, string Message)> DeleteFeatureAsync(int id)
        {
            var feature = await _featureRepo.GetByIdAsync(id);
            if (feature == null) return (false, "Không tìm thấy tính năng này!");

            // 1. KIỂM TRA RÀNG BUỘC DỮ LIỆU
            var carNames = await _featureRepo.GetCarNamesUsingFeatureAsync(id);
            if (carNames.Any())
            {
                // Nối tên các xe lại thành 1 chuỗi: "VinFast VF8, VinFast VF9"
                var carsList = string.Join(", ", carNames);

                // Trả về false và đính kèm câu cảnh báo cực gắt
                return (false, $"CẢNH BÁO: Không thể xóa! Tính năng này đang được trang bị trên các xe: [{carsList}]. Lời khuyên: Hãy dùng chức năng 'Sửa' (Cập nhật) thay vì xóa để không làm hỏng dữ liệu của xe.");
            }

            // 2. Nếu an toàn (Không có xe nào dùng) thì tiến hành xóa DB
            await _featureRepo.DeleteAsync(id);

            // 3. Dọn rác ảnh trong ổ cứng
            if (!string.IsNullOrEmpty(feature.Icon))
            {
                FileHelper.DeleteFile(feature.Icon);
            }

            return (true, "Đã xóa tính năng và dọn sạch ảnh thành công!");
        }

    }
}