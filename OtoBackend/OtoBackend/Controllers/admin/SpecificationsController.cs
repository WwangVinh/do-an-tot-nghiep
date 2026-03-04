using Microsoft.AspNetCore.Mvc;
using LogicBusiness.Interfaces.Admin;

namespace OtoBackend.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class SpecificationsController : ControllerBase
    {
        private readonly ICarSpecificationService _specService;

        public SpecificationsController(ICarSpecificationService specService)
        {
            _specService = specService;
        }

        // API này FE sẽ gọi để làm gợi ý lúc Admin gõ chữ
        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSuggestions()
        {
            var suggestions = await _specService.GetSuggestedSpecNamesAsync();
            return Ok(suggestions);
        }
    }
}