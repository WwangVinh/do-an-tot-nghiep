using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Interfaces.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicBusiness.Services.Admin
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepo;
        private readonly INotificationService _notificationService; 

        public ArticleService(IArticleRepository articleRepo, INotificationService notificationService)
        {
            _articleRepo = articleRepo;
            _notificationService = notificationService;
        }

        // 1. TẠO BÀI VIẾT MỚI
        public async Task<(bool Success, string Message)> CreateArticleAsync(ArticleSubmitDto dto, int authorId)
        {
            var article = new Article
            {
                Title = dto.Title,
                Thumbnail = dto.Thumbnail,
                Content = dto.Content,
                AuthorId = authorId,
                IsPublished = dto.IsPublished,
                CreatedAt = DateTime.Now
            };

            await _articleRepo.AddArticleAsync(article, dto.RelatedCarIds);

            await _notificationService.CreateNotificationAsync(
                userId: null,
                showroomId: null,
                roleTarget: "Marketing", // Gửi cho tất cả nhân viên Marketing
                title: "📰 Bài viết mới vừa đăng",
                content: $"Bài viết: \"{article.Title}\" đã được xuất bản.",
                actionUrl: "/news",
                type: "ARTICLE"
            );
            return (true, "Đăng bài viết mới thành công!");
        }

        // 2. CẬP NHẬT BÀI VIẾT 🚀
        public async Task<(bool Success, string Message)> UpdateArticleAsync(int articleId, ArticleSubmitDto dto)
        {
            var article = await _articleRepo.GetArticleByIdAsync(articleId);
            if (article == null) return (false, "Không tìm thấy bài viết để cập nhật!");

            // Cập nhật thông tin cơ bản
            article.Title = dto.Title;
            article.Thumbnail = dto.Thumbnail;
            article.Content = dto.Content;
            article.IsPublished = dto.IsPublished;

            // Chuyền xuống Repo để xử lý đồng bộ bảng trung gian ArticleCars
            await _articleRepo.UpdateArticleAsync(article, dto.RelatedCarIds);
            return (true, "Cập nhật bài viết thành công!");
        }

        // 3. XÓA BÀI VIẾT
        public async Task<(bool Success, string Message)> DeleteArticleAsync(int articleId)
        {
            var article = await _articleRepo.GetArticleByIdAsync(articleId);
            if (article == null) return (false, "Bài viết không tồn tại hoặc đã bị xóa trước đó!");

            await _articleRepo.DeleteArticleAsync(articleId);
            return (true, "Đã xóa bài viết khỏi hệ thống!");
        }

        // 4. LẤY DANH SÁCH CHO ADMIN (Có phân trang)
        public async Task<object> GetArticlesAdminAsync(string? search, int page, int pageSize, bool? isPublished)
        {
            var (articles, totalCount) = await _articleRepo.GetFilteredArticlesAsync(search, page, pageSize, isPublished);

            var articleDtos = articles.Select(a => new ArticleResponseDto
            {
                ArticleId = a.ArticleId,
                Title = a.Title,
                Thumbnail = a.Thumbnail,
                IsPublished = a.IsPublished,
                CreatedAt = a.CreatedAt
                // Ní có thể thêm FullName tác giả nếu Repo đã Include Author
            });

            return new
            {
                Data = articleDtos,
                TotalCount = totalCount
            };
        }

        // 5. CHI TIẾT BÀI VIẾT (Kèm dàn xe liên quan để hiện lên UI) 🏎️
        public async Task<ArticleDetailResponseDto?> GetArticleDetailAsync(int id)
        {
            var article = await _articleRepo.GetArticleByIdAsync(id);
            if (article == null) return null;

            return new ArticleDetailResponseDto
            {
                ArticleId = article.ArticleId,
                Title = article.Title,
                Content = article.Content,
                Thumbnail = article.Thumbnail,
                CreatedAt = article.CreatedAt,
                IsPublished = article.IsPublished,

                // Map danh sách xe liên quan từ ArticleCars sang DTO đơn giản
                RelatedCars = article.ArticleCars.Select(ac => new RelatedCarDto
                {
                    CarId = ac.Car.CarId,
                    Name = ac.Car.Name,
                    Price = ac.Car.Price,
                    ImageUrl = ac.Car.ImageUrl
                }).ToList()
            };
        }
    }
}