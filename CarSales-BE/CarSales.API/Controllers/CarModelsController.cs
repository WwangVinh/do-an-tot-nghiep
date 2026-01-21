using CarSales.API.Models;
using CarSales.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarSales.API.Controllers      //Bảng Dòng xe (Models) - VD: Camry, VF8, C-Class
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarModelsController : ControllerBase
    {
        private readonly ICarModelService _service;

        public CarModelsController(ICarModelService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarModel>>> GetCarModels()
        {
            return await _service.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CarModel>> GetCarModel(int id)
        {
            var carModel = await _service.GetByIdAsync(id);
            if (carModel == null) return NotFound();
            return carModel;
        }

        [HttpPost]
        public async Task<ActionResult<CarModel>> PostCarModel(CarModel carModel)
        {
            var created = await _service.CreateAsync(carModel);
            return CreatedAtAction(nameof(GetCarModel), new { id = created.ModelId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCarModel(int id, CarModel carModel)
        {
            var result = await _service.UpdateAsync(id, carModel);
            if (!result) return BadRequest();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCarModel(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
