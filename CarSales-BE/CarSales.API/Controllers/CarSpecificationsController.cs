using CarSales.API.Models;
using CarSales.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarSales.API.Controllers      //Bảng Giá trị Thông số của từng xe (CarSpecifications)
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarSpecificationsController : ControllerBase
    {
        private readonly ICarSpecificationService _service;

        public CarSpecificationsController(ICarSpecificationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarSpecification>>> GetCarSpecifications()
        {
            return await _service.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CarSpecification>> GetCarSpecification(int id)
        {
            var carSpecification = await _service.GetByIdAsync(id);
            if (carSpecification == null) return NotFound();
            return carSpecification;
        }

        [HttpPost]
        public async Task<ActionResult<CarSpecification>> PostCarSpecification(CarSpecification carSpecification)
        {
            var created = await _service.CreateAsync(carSpecification);
            return CreatedAtAction(nameof(GetCarSpecification), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCarSpecification(int id, CarSpecification carSpecification)
        {
            var result = await _service.UpdateAsync(id, carSpecification);
            if (!result) return BadRequest();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCarSpecification(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
