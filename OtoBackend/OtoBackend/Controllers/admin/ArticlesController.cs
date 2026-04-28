using Microsoft.AspNetCore.Authorization; // Hết lỗi Authorize và AuthorizeAttribute
using Microsoft.AspNetCore.Mvc;
using LogicBusiness.DTOs; // Hết lỗi ArticleSubmitDto
using System.Security.Claims;
using LogicBusiness.Interfaces.Admin;

namespace OtoBackend.Controllers.admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager},{AppRoles.Sales},{AppRoles.ShowroomSales},{AppRoles.Content},{AppRoles.Marketing}")]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _articleService;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ArticleSubmitDto dto)
        {
            // Bóc UserId từ Token của nhân viên đang đăng bài
            var authorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var result = await _articleService.CreateArticleAsync(dto, authorId);
            return Ok(result);
        }
    }
}
