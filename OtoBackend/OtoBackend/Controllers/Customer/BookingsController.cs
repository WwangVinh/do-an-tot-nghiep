using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OtoBackend.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Bắt buộc đăng nhập mới được "book" xe nhe ní
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // 1. ĐẶT LỊCH HẸN
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] BookingCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Lấy UserId từ Token ra cho chắc, đừng tin FE gửi lên
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            dto.UserId = int.Parse(userIdStr);

            var result = await _bookingService.CreateBookingAsync(dto);
            if (result.Success) return Ok(new { message = result.Message });

            return BadRequest(new { message = result.Message });
        }

        // 1b. ĐẶT LỊCH HẸN (KHÔNG CẦN ĐĂNG NHẬP)
        [AllowAnonymous]
        [HttpPost("public-create")]
        public async Task<IActionResult> PublicCreate([FromBody] BookingCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Khách vãng lai: không gắn UserId
            dto.UserId = null;

            var result = await _bookingService.CreateBookingAsync(dto);
            if (result.Success) return Ok(new { message = result.Message });

            return BadRequest(new { message = result.Message });
        }

        // 2. XEM LỊCH CỦA TÔI
        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetMyHistory()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var bookings = await _bookingService.GetMyBookingsAsync(int.Parse(userIdStr));
            return Ok(bookings);
        }

        // 3. KHÁCH TỰ HỦY LỊCH
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var result = await _bookingService.CancelBookingAsync(id, int.Parse(userIdStr));
            if (result.Success) return Ok(new { message = result.Message });

            return BadRequest(new { message = result.Message });
        }
    }
}