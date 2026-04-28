using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace OtoBackend.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager},{AppRoles.Marketing},{AppRoles.Sales}")] 
    public class SystemSettingsController : ControllerBase
    {
        private readonly ISystemSettingAdminService _service;

        public SystemSettingsController(ISystemSettingAdminService service)
        {
            _service = service;
        }

        // GET: api/admin/SystemSettings/DepositPercentage
        [HttpGet("{key}")]
        public async Task<IActionResult> GetSetting(string key)
        {
            var value = await _service.GetSettingValueAsync(key);
            if (value == null) return NotFound(new { message = "Không tìm thấy cấu hình này!" });

            return Ok(new { key = key, value = value });
        }

        // PUT: api/admin/SystemSettings/DepositPercentage
        [HttpPut("{key}")]
        public async Task<IActionResult> UpdateSetting(string key, [FromBody] SystemSettingUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SettingValue))
            {
                return BadRequest(new { message = "Giá trị không được để trống!" });
            }

            var result = await _service.UpdateSettingAsync(key, dto.SettingValue);

            if (!result.Success) return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }
    }
}