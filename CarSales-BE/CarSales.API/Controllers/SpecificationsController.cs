using CarSales.API.Models;
using CarSales.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarSales.API.Controllers      //Bảng Danh mục Thông số (Specifications)
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecificationsController : ControllerBase
    {
        private readonly ISpecificationService _service;

        public SpecificationsController(ISpecificationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Specification>>> GetSpecifications()
        {
            return await _service.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Specification>> GetSpecification(int id)
        {
            var specification = await _service.GetByIdAsync(id);
            if (specification == null) return NotFound();
            return specification;
        }

        [HttpPost]
        public async Task<ActionResult<Specification>> PostSpecification(Specification specification)
        {
            var created = await _service.CreateAsync(specification);
            return CreatedAtAction(nameof(GetSpecification), new { id = created.SpecId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpecification(int id, Specification specification)
        {
            var result = await _service.UpdateAsync(id, specification);
            if (!result) return BadRequest();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpecification(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
