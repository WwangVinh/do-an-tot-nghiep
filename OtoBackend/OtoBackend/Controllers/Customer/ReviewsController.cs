using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Customer;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;


namespace OtoBackend.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // GET /api/Reviews/verify-phone?phone=xxx&carId=11
        [HttpGet("verify-phone")]
        public async Task<IActionResult> VerifyPhone([FromQuery] string phone, [FromQuery] int carId)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return BadRequest(new { message = "Số điện thoại không được để trống." });

            var result = await _reviewService.CheckReviewEligibilityAsync(phone.Trim(), carId);
            if (!result.IsEligible)
                return BadRequest(new { message = result.Message });

            return Ok(new { fullName = result.FullName });
        }

        // POST /api/Reviews/submit
        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody] ReviewSubmitDto dto)
        {
            try
            {
                var result = await _reviewService.SubmitReviewAsync(dto);
                if (!result.Success) return BadRequest(new { message = result.Message });
                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }

        // GET /api/Reviews/car/{carId} — public, không cần auth
        [HttpGet("car/{carId}")]
        public async Task<IActionResult> GetByCar(int carId)
        {
            try
            {
                var reviews = await _reviewService.GetApprovedReviewsByCarIdAsync(carId);
                // Map sang anonymous object để tránh circular reference với navigation Car
                var result = reviews.Select(r => new
                {
                    r.ReviewId,
                    r.CarId,
                    r.FullName,
                    r.Rating,
                    r.Comment,
                    r.CreatedAt,
                    r.IsApproved
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
            }
        }
    }
}