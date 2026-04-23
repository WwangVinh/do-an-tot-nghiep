using LogicBusiness.DTOs;

namespace LogicBusiness.Interfaces.Customer
{
    public interface IPricingService
    {
        Task<IEnumerable<PricingCarDto>> GetPricingCarsAsync();
    }
}
