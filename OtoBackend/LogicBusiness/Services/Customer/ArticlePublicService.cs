using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Customer;
using LogicBusiness.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Services.Customer
{
    public class ArticlePublicService : IArticlePublicService
    {
        private readonly IArticleRepository _articleRepo;

        public ArticlePublicService(IArticleRepository articleRepo)
        {
            _articleRepo = articleRepo;
        }

        // Danh sách bài viết public — chỉ lấy IsPublished = true, hỗ trợ search + phân trang
        public async Task<ArticlePublicListResult> GetPublishedArticlesAsync(string? search, int page, int pageSize)
        {
            // Đảm bảo page và pageSize hợp lệ
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            // Tái sử dụng repo có sẵn, cố định isPublished = true
            var (articles, totalCount) = await _articleRepo.GetFilteredArticlesAsync(
                search, page, pageSize, isPublished: true);

            var data = articles.Select(a => new ArticleResponseDto
            {
                ArticleId = a.ArticleId,
                Title = a.Title,
                Thumbnail = a.Thumbnail,
                IsPublished = a.IsPublished,
                CreatedAt = a.CreatedAt,
            });

            return new ArticlePublicListResult
            {
                Data = data,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
            };
        }

        // Chi tiết bài viết — chỉ trả về nếu IsPublished = true, tránh lộ bản nháp
        public async Task<ArticleDetailResponseDto?> GetPublishedArticleDetailAsync(int id)
        {
            var article = await _articleRepo.GetArticleByIdAsync(id);

            if (article == null || !article.IsPublished)
                return null;

            return new ArticleDetailResponseDto
            {
                ArticleId = article.ArticleId,
                Title = article.Title,
                Content = article.Content,
                Thumbnail = article.Thumbnail,
                CreatedAt = article.CreatedAt,
                IsPublished = article.IsPublished,
                RelatedCars = article.ArticleCars.Select(ac => new RelatedCarDto
                {
                    CarId = ac.Car.CarId,
                    Name = ac.Car.Name,
                    Price = ac.Car.Price,
                    ImageUrl = ac.Car.ImageUrl,
                }).ToList(),
            };
        }
    }
}
