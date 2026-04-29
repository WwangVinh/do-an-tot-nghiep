using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OtoBackend.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales},{AppRoles.Technician}")]
    public class PricingVersionsController : ControllerBase
    {
        private readonly IPricingAdminService _pricingAdminService;

    public PricingVersionsController(IPricingAdminService pricingAdminService)
    {
        _pricingAdminService = pricingAdminService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? carId = null, [FromQuery] bool? isActive = null)
    {
        var data = await _pricingAdminService.GetAllAsync(carId, isActive);
        return Ok(new { message = "Lấy danh sách bảng giá thành công!", data });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PricingVersionUpsertDto dto)
    {
        var result = await _pricingAdminService.CreateAsync(dto);
        if (!result.IsSuccess) return BadRequest(new { message = result.Message });
        return Ok(new { message = result.Message, data = result.Data });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PricingVersionUpsertDto dto)
    {
        var result = await _pricingAdminService.UpdateAsync(id, dto);
        if (!result.IsSuccess) return BadRequest(new { message = result.Message });
        return Ok(new { message = result.Message, data = result.Data });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _pricingAdminService.DeleteAsync(id);
        if (!result.IsSuccess) return BadRequest(new { message = result.Message });
        return Ok(new { message = result.Message });
    }
}
}
