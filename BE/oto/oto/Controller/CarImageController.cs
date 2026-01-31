using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oto.MyModels;

namespace oto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarImageController : ControllerBase
    {
        private readonly OtoContext _context;

        public CarImageController(OtoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarImage>>> GetCarImages()
        {
            return await _context.CarImages
                .Include(ci => ci.Car)
                .OrderByDescending(ci => ci.CreatedAt)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<CarImage>> PostCarImage(
            [FromForm] int carId,
            [FromForm] string imageUrl,
            [FromForm] int isMainImage) // Vue gửi 1 hoặc 0
        {
            var carImage = new CarImage
            {
                CarId = carId,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.Now,
                // SỬA LỖI: Gán trực tiếp kiểu bool (true/false) vào IsMainImage
                IsMainImage = isMainImage == 1 
            };

            // Nếu đây là ảnh chính, tìm các ảnh khác của xe này và đặt chúng thành false
            if (isMainImage == 1)
            {
                var others = await _context.CarImages
                    .Where(ci => ci.CarId == carId && ci.IsMainImage == true)
                    .ToListAsync();
                foreach (var img in others) img.IsMainImage = false;
            }

            _context.CarImages.Add(carImage);
            await _context.SaveChangesAsync();
            return Ok(carImage);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCarImage(int id,
            [FromForm] int? carId,
            [FromForm] string? imageUrl,
            [FromForm] int? isMainImage)
        {
            var carImage = await _context.CarImages.FindAsync(id);
            if (carImage == null) return NotFound();

            if (carId.HasValue) carImage.CarId = carId.Value;
            if (!string.IsNullOrEmpty(imageUrl)) carImage.ImageUrl = imageUrl;
            
            if (isMainImage.HasValue)
            {
                // SỬA LỖI: Cập nhật giá trị bool
                carImage.IsMainImage = isMainImage.Value == 1;

                if (isMainImage.Value == 1)
                {
                    var cid = carId ?? carImage.CarId;
                    var others = await _context.CarImages
                        .Where(ci => ci.CarId == cid && ci.CarImageId != id && ci.IsMainImage == true)
                        .ToListAsync();
                    foreach (var img in others) img.IsMainImage = false;
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCarImage(int id)
        {
            var carImage = await _context.CarImages.FindAsync(id);
            if (carImage == null) return NotFound();
            _context.CarImages.Remove(carImage);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}