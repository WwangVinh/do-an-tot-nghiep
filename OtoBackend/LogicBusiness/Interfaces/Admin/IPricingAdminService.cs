using LogicBusiness.DTOs;

namespace LogicBusiness.Interfaces.Admin
{
    public interface IPricingAdminService
    {
        Task<IEnumerable<PricingVersionAdminDto>> GetAllAsync(int? carId = null, bool? isActive = null);
        Task<(bool IsSuccess, string Message, PricingVersionAdminDto? Data)> CreateAsync(PricingVersionUpsertDto dto);
        Task<(bool IsSuccess, string Message, PricingVersionAdminDto? Data)> UpdateAsync(int id, PricingVersionUpsertDto dto);
        Task<(bool IsSuccess, string Message)> DeleteAsync(int id);
    }
}
