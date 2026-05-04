using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace OtoBackend.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class SystemSettingsController : ControllerBase
    {
        private readonly ISystemSettingAdminService _service;

        public SystemSettingsController(ISystemSettingAdminService service)
        {
            _service = service;
        }

        // ✅ Public — FE đọc % đặt cọc không cần đăng nhập
        [HttpGet("public/{key}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublicSetting(string key)
        {
            // Chỉ cho phép đọc các key an toàn
            var allowedKeys = new[] { "DepositPercentage", "FrontendUrl" };
            if (!System.Array.Exists(allowedKeys, k => k == key))
                return Forbid();

            var value = await _service.GetSettingValueAsync(key);
            if (value == null) return NotFound(new { message = "Không tìm thấy cấu hình này!" });
            return Ok(new { key, value });
        }

        // Admin only
        [HttpGet("{key}")]
        [Authorize(Roles = "Admin,Manager,Marketing,Sales")]
        public async Task<IActionResult> GetSetting(string key)
        {
            var value = await _service.GetSettingValueAsync(key);
            if (value == null) return NotFound(new { message = "Không tìm thấy cấu hình này!" });
            return Ok(new { key, value });
        }

        [HttpPut("{key}")]
        [Authorize(Roles = "Admin,Manager,Marketing,Sales")]
        public async Task<IActionResult> UpdateSetting(string key, [FromBody] SystemSettingUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SettingValue))
                return BadRequest(new { message = "Giá trị không được để trống!" });

            if (key == "DepositPercentage")
            {
                if (!decimal.TryParse(dto.SettingValue, out decimal percentage))
                    return BadRequest(new { message = "Phần trăm đặt cọc phải là một con số hợp lệ!" });
                if (percentage < 0 || percentage > 100)
                    return BadRequest(new { message = "Phần trăm đặt cọc phải nằm trong khoảng từ 0 đến 100." });
            }

            var result = await _service.UpdateSettingAsync(key, dto.SettingValue);
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }
    }
}