using LogicBusiness.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Customer
{
    public interface IArticlePublicService
    {
        /// <summary>
        /// Lấy danh sách bài viết đã publish, có hỗ trợ tìm kiếm theo từ khóa và phân trang.
        /// </summary>
        Task<ArticlePublicListResult> GetPublishedArticlesAsync(string? search, int page, int pageSize);

        /// <summary>
        /// Lấy chi tiết một bài viết đã publish kèm danh sách xe liên quan.
        /// </summary>
        Task<ArticleDetailResponseDto?> GetPublishedArticleDetailAsync(int id);
    }
}
