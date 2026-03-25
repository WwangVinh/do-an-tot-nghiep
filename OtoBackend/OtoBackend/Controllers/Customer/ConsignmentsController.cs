using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Interfaces.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OtoBackend.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // 👈 Bắt buộc phải đăng nhập
    public class ConsignmentsController : ControllerBase
    {
        private readonly IConsignmentService _consignService;
        private readonly IConsignmentRepository _consignRepo;

        public ConsignmentsController(IConsignmentService consignService, IConsignmentRepository consignRepo)
        {
            _consignService = consignService;
            _consignRepo = consignRepo;
        }

        // 1. KHÁCH NỘP ĐƠN KÝ GỬI
        [HttpPost]
        public async Task<IActionResult> CreateConsignment([FromBody] ConsignmentCreateDto dto)
        {
            // Bóc tách ID và Tên của khách hàng từ Token (JWT)
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                           ?? User.Claims.FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized(new { Success = false, Message = "Không xác định được danh tính người dùng!" });

            var customerName = User.FindFirstValue(ClaimTypes.Name) ?? "Khách hàng";

            var result = await _consignService.CreateConsignmentAsync(userId, customerName, dto);

            if (!result.Success) return BadRequest(new { Success = false, Message = result.Message });
            return Ok(new { Success = true, Message = result.Message });
        }

        // 2. KHÁCH XEM LẠI DANH SÁCH XE ĐÃ NỘP CỦA MÌNH
        [HttpGet("my-consignments")]
        public async Task<IActionResult> GetMyConsignments()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                           ?? User.Claims.FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();

            var consignments = await _consignRepo.GetByUserIdAsync(userId);

            // Map sang DTO cho sạch sẽ
            var result = consignments.Select(c => new
            {
                c.ConsignmentId,
                c.Brand,
                c.Model,
                c.Year,
                c.ExpectedPrice,
                c.AgreedPrice,
                c.Status,
                CreatedAt = c.CreatedAt?.ToString("dd/MM/yyyy HH:mm") ?? ""
            });

            return Ok(new { Success = true, Data = result });
        }
    }
}