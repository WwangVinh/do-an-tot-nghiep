using CarSales.API.Models;
using CarSales.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarSales.API.Controllers      //Bảng Hình ảnh xe (CarImages) - Thư viện ảnh
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarImagesController : ControllerBase
    {
        private readonly ICarImageService _service;

        public CarImagesController(ICarImageService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarImage>>> GetCarImages()
        {
            return await _service.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CarImage>> GetCarImage(int id)
        {
            var carImage = await _service.GetByIdAsync(id);
            if (carImage == null) return NotFound();
            return carImage;
        }

        [HttpPost]
        public async Task<ActionResult<CarImage>> PostCarImage(CarImage carImage)
        {
            var created = await _service.CreateAsync(carImage);
            return CreatedAtAction(nameof(GetCarImage), new { id = created.ImageId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCarImage(int id, CarImage carImage)
        {
            var result = await _service.UpdateAsync(id, carImage);
            if (!result) return BadRequest();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCarImage(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
