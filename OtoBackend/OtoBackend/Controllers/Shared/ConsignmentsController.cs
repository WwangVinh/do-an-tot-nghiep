using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OtoBackend.Controllers.Shared
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsignmentsController : ControllerBase
    {
        private readonly IConsignmentService _consignService;

        public ConsignmentsController(IConsignmentService consignService)
        {
            _consignService = consignService;
        }

        // CUSTOMER
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateConsignment([FromBody] ConsignmentCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Success = false, Errors = ModelState });

            var result = await _consignService.CreateConsignmentAsync(dto);

            if (!result.Success) return BadRequest(new { Success = false, result.Message });
            return Ok(new { Success = true, result.Message });
        }

        // ADMIN / MANAGER / SALES
        [HttpGet("admin")]
        [Authorize(Roles = "Admin,Manager,Sales,ShowroomSales")]
        public async Task<IActionResult> GetAllConsignments()
        {
            var data = await _consignService.GetAllAdminAsync();
            return Ok(new { Success = true, Data = data });
        }

        [HttpGet("admin/{id:int}")]
        [Authorize(Roles = "Admin,Manager,Sales,ShowroomSales")]
        public async Task<IActionResult> GetConsignmentDetail(int id)
        {
            var data = await _consignService.GetByIdAsync(id);
            if (data == null)
                return NotFound(new { Success = false, Message = "Không tìm thấy hồ sơ." });

            return Ok(new { Success = true, Data = data });
        }

        [HttpPut("admin/{id:int}")]
        [Authorize(Roles = "Admin,Manager,Sales,ShowroomSales")]
        public async Task<IActionResult> UpdateConsignment(int id, [FromBody] ConsignmentUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Success = false, Errors = ModelState });

            var role = User.FindFirstValue(ClaimTypes.Role) ?? "";
            var result = await _consignService.UpdateConsignmentStatusAsync(id, dto, role);

            if (!result.Success) return BadRequest(new { Success = false, result.Message });
            return Ok(new { Success = true, result.Message });
        }
    }
}