using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Customer;
using Microsoft.AspNetCore.Mvc;


namespace OtoBackend.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase // Chỉ giữ lại class này thôi
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // Khách gửi đánh giá
        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody] ReviewSubmitDto dto)
        {
            var result = await _reviewService.SubmitReviewAsync(dto);
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        // Lấy đánh giá của một xe (Hiện ra ngoài trang chủ/chi tiết xe)
        [HttpGet("car/{carId}")]
        public async Task<IActionResult> GetByCar(int carId)
        {
            var reviews = await _reviewService.GetApprovedReviewsByCarIdAsync(carId);
            return Ok(reviews);
        }
    }
}
