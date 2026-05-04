using LogicBusiness.Interfaces.Admin;
using Microsoft.AspNetCore.Mvc;

namespace OtoBackend.Controllers.Customer
{
    // ── Public: khách lấy phụ kiện của xe để chọn khi đặt mua ───────────────
    [Route("api/public/cars/{carId}/accessories")]
    [ApiController]
    public class CarAccessoriesPublicController : ControllerBase
    {
        private readonly IAccessoryService _service;

        public CarAccessoriesPublicController(IAccessoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetByCarId(int carId)
            => Ok(await _service.GetByCarIdAsync(carId));
    }
}