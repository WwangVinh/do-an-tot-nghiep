using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Customer;
using Microsoft.AspNetCore.Mvc;

namespace OtoBackend.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] BookingCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.UserId = null;
            var result = await _bookingService.CreateBookingAsync(dto);
            if (result.Success) return Ok(new { message = result.Message });
            return BadRequest(new { message = result.Message });
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> Lookup([FromQuery] string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return BadRequest(new { message = "Vui lòng nhập số điện thoại." });
            var bookings = await _bookingService.GetBookingsByPhoneAsync(phone.Trim());
            return Ok(bookings);
        }

        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetDetail(int id, [FromQuery] string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return BadRequest(new { message = "Vui lòng nhập số điện thoại để xác thực." });
            var detail = await _bookingService.GetBookingDetailByPhoneAsync(id, phone.Trim());
            if (detail == null) return NotFound(new { message = "Không tìm thấy lịch hẹn hoặc số điện thoại không khớp." });
            return Ok(detail);
        }

        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> Cancel(int id, [FromBody] BookingCancelByPhoneDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _bookingService.CancelBookingByPhoneAsync(id, dto.Phone.Trim(), dto.Reason);
            if (result.Success) return Ok(new { message = result.Message });
            return BadRequest(new { message = result.Message });
        }
    }
}