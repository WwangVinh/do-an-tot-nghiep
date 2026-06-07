using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Customer;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace OtoBackend.Controllers
{
    [ApiController]
    [Route("api/consult-requests")]
    public class ConsultRequestController : ControllerBase
    {
        private readonly IConsultRequestService _service;

        public ConsultRequestController(IConsultRequestService service)
        {
            _service = service;
        }

        // ============================================================
        // POST: api/consult-requests
        // Khách gửi yêu cầu báo giá / mua trả góp
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ConsultRequestCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate cơ bản
            if (string.IsNullOrWhiteSpace(dto.CustomerName))
                return BadRequest(new { success = false, message = "Vui lòng nhập họ tên." });
            if (string.IsNullOrWhiteSpace(dto.Phone))
                return BadRequest(new { success = false, message = "Vui lòng nhập số điện thoại." });
            if (dto.CarId <= 0)
                return BadRequest(new { success = false, message = "Vui lòng chọn xe." });
            if (dto.ShowroomId <= 0)
                return BadRequest(new { success = false, message = "Vui lòng chọn showroom." });

            var (success, message) = await _service.CreateAsync(dto);
            if (!success)
                return BadRequest(new { success, message });

            return Ok(new { success, message });
        }

        // ============================================================
        // GET: api/consult-requests/by-phone/{phone}
        // Khách xem danh sách yêu cầu của mình theo SĐT
        // ============================================================
        [HttpGet("by-phone/{phone}")]
        public async Task<IActionResult> GetByPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return BadRequest(new { success = false, message = "Vui lòng nhập số điện thoại." });

            var items = await _service.GetByPhoneAsync(phone.Trim());
            return Ok(new { success = true, data = items });
        }

        // ============================================================
        // GET: api/consult-requests/{id}?phone=0901234567
        // Khách xem chi tiết 1 yêu cầu (cần SĐT để verify)
        // ============================================================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetail(int id, [FromQuery] string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return BadRequest(new { success = false, message = "Vui lòng cung cấp số điện thoại để xem chi tiết." });

            var detail = await _service.GetDetailByPhoneAsync(id, phone.Trim());
            if (detail == null)
                return NotFound(new { success = false, message = "Không tìm thấy yêu cầu hoặc số điện thoại không khớp." });

            return Ok(new { success = true, data = detail });
        }

        // ============================================================
        // PUT: api/consult-requests/{id}/cancel
        // Khách tự hủy yêu cầu (chỉ khi còn ở Pending)
        // ============================================================
        [HttpPut("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id, [FromBody] ConsultCancelByPhoneDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Phone))
                return BadRequest(new { success = false, message = "Vui lòng cung cấp số điện thoại." });

            var (success, message) = await _service.CancelByPhoneAsync(id, dto.Phone.Trim(), dto.Reason);
            if (!success)
                return BadRequest(new { success, message });

            return Ok(new { success, message });
        }
    }
}