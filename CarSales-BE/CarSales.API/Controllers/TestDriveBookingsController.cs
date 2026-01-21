using CarSales.API.Models;
using CarSales.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarSales.API.Controllers      //Bảng Đặt lịch lái thử (TestDriveBookings)
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestDriveBookingsController : ControllerBase
    {
        private readonly ITestDriveBookingService _service;

        public TestDriveBookingsController(ITestDriveBookingService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TestDriveBooking>>> GetTestDriveBookings()
        {
            return await _service.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TestDriveBooking>> GetTestDriveBooking(int id)
        {
            var booking = await _service.GetByIdAsync(id);
            if (booking == null) return NotFound();
            return booking;
        }

        [HttpPost]
        public async Task<ActionResult<TestDriveBooking>> PostTestDriveBooking(TestDriveBooking booking)
        {
            var created = await _service.CreateAsync(booking);
            return CreatedAtAction(nameof(GetTestDriveBooking), new { id = created.BookingId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTestDriveBooking(int id, TestDriveBooking booking)
        {
            var result = await _service.UpdateAsync(id, booking);
            if (!result) return BadRequest();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTestDriveBooking(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
