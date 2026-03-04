using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace OtoBackend.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class FeaturesController : ControllerBase
    {
        private readonly IFeatureService _featureService;

        public FeaturesController(IFeatureService featureService)
        {
            _featureService = featureService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search = null)
        {
            // Truyền chữ search mà Admin gõ vào Service
            var features = await _featureService.GetAllFeaturesAsync(search);
            return Ok(features);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] FeatureCreateDto dto)
        {
            var result = await _featureService.CreateFeatureAsync(dto);
            if (result.IsSuccess) return Ok(new { message = result.Message });

            return BadRequest(new { message = result.Message }); // Trả về câu "Trùng tên rồi..."
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] FeatureUpdateDto dto)
        {
            var result = await _featureService.UpdateFeatureAsync(id, dto);
            if (result.IsSuccess) return Ok(new { message = result.Message });

            return BadRequest(new { message = result.Message }); // Trả về câu "Trùng tên rồi..."
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Nhận về Tuple gồm trạng thái (IsSuccess) và Lời nhắn (Message)
            var result = await _featureService.DeleteFeatureAsync(id);

            // Nếu xóa thành công
            if (result.IsSuccess)
            {
                return Ok(new { message = result.Message });
            }

            // Nếu bị chặn (hoặc lỗi)
            return BadRequest(new { message = result.Message });
        }
    }
}