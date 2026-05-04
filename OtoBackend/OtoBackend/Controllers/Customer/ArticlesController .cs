using LogicBusiness.Interfaces.Customer;
using Microsoft.AspNetCore.Mvc;

namespace YourApi.Controllers.Public
{
    [ApiController]
    [Route("api/public/articles")]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticlePublicService _articlePublicService;

        public ArticlesController(IArticlePublicService articlePublicService)
        {
            _articlePublicService = articlePublicService;
        }

        /// <summary>
        /// GET /api/public/articles
        /// Lấy danh sách bài viết đã đăng, hỗ trợ tìm kiếm và phân trang.
        /// Không yêu cầu đăng nhập.
        /// </summary>
        /// <param name="search">Từ khóa tìm kiếm trong tiêu đề (optional)</param>
        /// <param name="page">Trang hiện tại, bắt đầu từ 1 (default: 1)</param>
        /// <param name="pageSize">Số bài mỗi trang, tối đa 50 (default: 10)</param>
        [HttpGet]
        public async Task<IActionResult> GetArticles(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _articlePublicService.GetPublishedArticlesAsync(search, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// GET /api/public/articles/{id}
        /// Lấy chi tiết một bài viết đã đăng kèm danh sách xe liên quan.
        /// Trả 404 nếu bài không tồn tại hoặc chưa publish.
        /// Không yêu cầu đăng nhập.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetArticleDetail([FromRoute] int id)
        {
            var article = await _articlePublicService.GetPublishedArticleDetailAsync(id);

            if (article == null)
                return NotFound(new { message = "Bài viết không tồn tại hoặc chưa được đăng." });

            return Ok(article);
        }
    }
}