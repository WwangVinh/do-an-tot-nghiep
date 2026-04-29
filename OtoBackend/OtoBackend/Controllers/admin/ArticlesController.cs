using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LogicBusiness.DTOs;
using System.Security.Claims;
using LogicBusiness.Interfaces.Admin;
using System.IdentityModel.Tokens.Jwt;

namespace OtoBackend.Controllers.Admin // Nhớ viết hoa chữ Admin cho chuẩn Convention nha ní
{
    [Route("api/admin/[controller]")]
    [ApiController]
    // Tui lọc lại quyền, bỏ mấy bạn Sales ra vì mấy bạn đó không phụ trách viết bài
    [Authorize(Roles = "Admin,Manager,Content,Marketing")]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _articleService;

        public ArticlesController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        // 1. LẤY DANH SÁCH BÀI VIẾT (Có phân trang, tìm kiếm)
        [HttpGet]
        public async Task<IActionResult> GetArticles(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12,
            [FromQuery] bool? isPublished = null)
        {
            var result = await _articleService.GetArticlesAdminAsync(search, page, pageSize, isPublished);
            return Ok(result);
        }

        // 2. XEM CHI TIẾT 1 BÀI VIẾT (Kèm dàn xe liên quan)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticleDetail(int id)
        {
            var result = await _articleService.GetArticleDetailAsync(id);
            if (result == null)
                return NotFound(new { message = "Không tìm thấy bài viết!" });

            return Ok(result);
        }

        // 3. ĐĂNG BÀI VIẾT MỚI
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ArticleSubmitDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Bóc UserId từ Token an toàn hơn (hỗ trợ cả 2 chuẩn Claim)
            string? claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            int authorId = string.IsNullOrEmpty(claimId) ? 0 : int.Parse(claimId);

            var result = await _articleService.CreateArticleAsync(dto, authorId);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        // 4. CẬP NHẬT BÀI VIẾT VÀ GẮN LẠI THẺ XE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ArticleSubmitDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _articleService.UpdateArticleAsync(id, dto);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        // 5. XÓA BÀI VIẾT
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _articleService.DeleteArticleAsync(id);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }
    }
}