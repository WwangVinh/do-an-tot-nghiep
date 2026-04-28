using LogicBusiness.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace OtoBackend.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannersController : ControllerBase
    {
        private readonly IBannerRepository _repo;

        public BannersController(IBannerRepository repo)
        {
            _repo = repo;
        }

        // GET: api/Banners?isActive=true
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? isActive = true)
        {
            var now = DateTime.UtcNow;
            var banners = await _repo.GetAllAsync(isActive);

            var visible = banners
                .Where(b =>
                    (!b.StartDate.HasValue || b.StartDate.Value.ToUniversalTime() <= now) &&
                    (!b.EndDate.HasValue || b.EndDate.Value.ToUniversalTime() >= now))
                .Select(b => new
                {
                    bannerId = b.BannerId,
                    imageUrl = b.ImageUrl,
                    linkUrl = b.LinkUrl,
                    position = b.Position,
                    isActive = b.IsActive,
                    bannerName = b.BannerName,
                    startDate = b.StartDate,
                    endDate = b.EndDate
                })
                .ToList();

            return Ok(new { message = "Lấy danh sách banner thành công!", data = visible });
        }
    }
}

