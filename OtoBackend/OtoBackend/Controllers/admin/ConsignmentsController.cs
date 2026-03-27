using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Interfaces.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OtoBackend.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin, ShowroomManager")] // 👈 Chỉ Sếp mới được vào đây chốt giá
    public class ConsignmentsController : ControllerBase
    {
        private readonly IConsignmentService _consignService;
        private readonly IConsignmentRepository _consignRepo;

        public ConsignmentsController(IConsignmentService consignService, IConsignmentRepository consignRepo)
        {
            _consignService = consignService;
            _consignRepo = consignRepo;
        }

        // 1. SẾP XEM TOÀN BỘ YÊU CẦU KÝ GỬI
        [HttpGet]
        public async Task<IActionResult> GetAllConsignments()
        {
            var consignments = await _consignRepo.GetAllAdminAsync();

            var result = consignments.Select(c => new ConsignmentResponseDto
            {
                ConsignmentId = c.ConsignmentId,
                UserId = c.UserId,
                CustomerName = c.User?.FullName ?? "Khách vãng lai",
                Brand = c.Brand,
                Model = c.Model,
                Year = c.Year,
                Mileage = c.Mileage,
                ExpectedPrice = c.ExpectedPrice ?? 0,
                AgreedPrice = c.AgreedPrice,
                CommissionRate = c.CommissionRate,
                Status = c.Status,
                LinkedCarId = c.LinkedCarId,
                CreatedAt = c.CreatedAt?.ToString("dd/MM/yyyy HH:mm") ?? ""
            });

            return Ok(new { Success = true, Data = result });
        }

        // 2. SẾP XEM CHI TIẾT 1 HỒ SƠ
        [HttpGet("{id}")]
        public async Task<IActionResult> GetConsignmentDetail(int id)
        {
            var c = await _consignRepo.GetByIdAsync(id);
            if (c == null) return NotFound(new { Success = false, Message = "Không tìm thấy hồ sơ." });

            var result = new ConsignmentResponseDto
            {
                ConsignmentId = c.ConsignmentId,
                UserId = c.UserId,
                CustomerName = c.User?.FullName ?? "Khách vãng lai",
                Brand = c.Brand,
                Model = c.Model,
                Year = c.Year,
                Mileage = c.Mileage,
                ConditionDescription = c.ConditionDescription,
                ExpectedPrice = c.ExpectedPrice?? 0,
                AgreedPrice = c.AgreedPrice,
                CommissionRate = c.CommissionRate,
                Status = c.Status,
                LinkedCarId = c.LinkedCarId,
                CreatedAt = c.CreatedAt?.ToString("dd/MM/yyyy HH:mm") ?? ""
            };

            return Ok(new { Success = true, Data = result });
        }

        // 3. SẾP CHỐT GIÁ & DUYỆT HỒ SƠ
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateConsignmentStatus(int id, [FromBody] ConsignmentUpdateDto dto)
        {
            // Bóc cái chức vụ (Role) của ông đang bấm nút ra
            var adminRole = User.FindFirstValue(ClaimTypes.Role) ?? "";

            var result = await _consignService.UpdateConsignmentStatusAsync(id, dto, adminRole);

            if (!result.Success) return BadRequest(new { Success = false, Message = result.Message });
            return Ok(new { Success = true, Message = result.Message });
        }
    }
}