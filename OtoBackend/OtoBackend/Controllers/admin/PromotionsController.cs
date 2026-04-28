using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace OtoBackend.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager},{AppRoles.Content},{AppRoles.Marketing},{AppRoles.Sales},{AppRoles.ShowroomSales}")]
    public class PromotionsController : ControllerBase
    {
        private readonly IPromotionAdminService _service;
        public PromotionsController(IPromotionAdminService service) { _service = service; }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllPromotionsAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PromotionCreateUpdateDto dto)
        {
            var result = await _service.CreatePromotionAsync(dto);
            return result.Success ? Ok(new { message = result.Message }) : BadRequest(new { message = result.Message });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PromotionCreateUpdateDto dto)
        {
            var result = await _service.UpdatePromotionAsync(id, dto);
            return result.Success ? Ok(new { message = result.Message }) : BadRequest(new { message = result.Message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeletePromotionAsync(id);
            return result.Success ? Ok(new { message = result.Message }) : BadRequest(new { message = result.Message });
        }
    }
}