using LogicBusiness.Interfaces.Customer;
using Microsoft.AspNetCore.Mvc;

namespace OtoBackend.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class PricingController : ControllerBase
    {
        private readonly IPricingService _pricingService;

        public PricingController(IPricingService pricingService)
        {
            _pricingService = pricingService;
        }

        [HttpGet("cars")]
        public async Task<IActionResult> GetPricingCars()
        {
            var data = await _pricingService.GetPricingCarsAsync();
            return Ok(new { message = "Lấy bảng giá xe thành công!", data });
        }
    }
}
