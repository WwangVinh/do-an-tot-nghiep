using CoreEntities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface IArticleRepository
    {
        // Lấy chi tiết bài viết kèm dữ liệu từ bảng trung gian ArticleCars
        Task<Article?> GetArticleByIdAsync(int id);

        // Lấy danh sách bài viết kèm phân trang cho Admin/Content
        Task<(IEnumerable<Article> Articles, int TotalCount)> GetFilteredArticlesAsync(
            string? search, int page, int pageSize, bool? isPublished);

        // Thêm bài viết mới và gắn thẻ các xe liên quan
        Task AddArticleAsync(Article article, List<int> carIds);

        // Đồng bộ hóa lại danh sách xe khi cập nhật bài viết
        Task UpdateArticleAsync(Article article, List<int> carIds);

        // Xóa bài viết vĩnh viễn khỏi hệ thống
        Task DeleteArticleAsync(int id);
    }
}