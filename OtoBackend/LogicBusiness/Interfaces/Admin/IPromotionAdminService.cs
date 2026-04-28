using LogicBusiness.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Admin
{
    public interface IPromotionAdminService
    {
        Task<IEnumerable<PromotionAdminDto>> GetAllPromotionsAsync();
        Task<(bool Success, string Message)> CreatePromotionAsync(PromotionCreateUpdateDto dto);
        Task<(bool Success, string Message)> UpdatePromotionAsync(int id, PromotionCreateUpdateDto dto);
        Task<(bool Success, string Message)> DeletePromotionAsync(int id);
    }
}
