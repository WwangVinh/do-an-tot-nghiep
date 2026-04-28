using LogicBusiness.DTOs;
using System.Threading.Tasks;


namespace LogicBusiness.Interfaces.Admin
{
    public interface IArticleService
    {
        // Tạo bài viết mới với ID tác giả bóc từ Token
        Task<(bool Success, string Message)> CreateArticleAsync(ArticleSubmitDto dto, int authorId);

        // Cập nhật nội dung bài viết và danh sách xe gắn kèm
        Task<(bool Success, string Message)> UpdateArticleAsync(int articleId, ArticleSubmitDto dto);

        // Xóa bài viết theo yêu cầu của Admin
        Task<(bool Success, string Message)> DeleteArticleAsync(int articleId);

        // Lấy danh sách bài viết đã bọc qua Object để trả về UI Admin
        Task<object> GetArticlesAdminAsync(string? search, int page, int pageSize, bool? isPublished);

        // Lấy chi tiết bài viết kèm DTO dàn xe liên quan để hiện lên giao diện tin tức
        Task<ArticleDetailResponseDto?> GetArticleDetailAsync(int id);
    }
}