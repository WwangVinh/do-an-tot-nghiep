using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlServer.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly OtoContext _context;

        public ArticleRepository(OtoContext context)
        {
            _context = context;
        }

        // 1. LẤY CHI TIẾT (Kèm danh sách xe đã link)
        public async Task<Article?> GetArticleByIdAsync(int id)
        {
            return await _context.Articles
                .Include(a => a.ArticleCars)
                .ThenInclude(ac => ac.Car)
                .FirstOrDefaultAsync(a => a.ArticleId == id);
        }

        // 2. LỌC VÀ PHÂN TRANG
        public async Task<(IEnumerable<Article> Articles, int TotalCount)> GetFilteredArticlesAsync(
            string? search, int page, int pageSize, bool? isPublished)
        {
            var query = _context.Articles.AsQueryable();

            if (isPublished.HasValue)
                query = query.Where(a => a.IsPublished == isPublished.Value);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(a => a.Title.Contains(search));

            int totalCount = await query.CountAsync();

            var articles = await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (articles, totalCount);
        }

        // 3. THÊM MỚI (Kèm gắn thẻ xe)
        public async Task AddArticleAsync(Article article, List<int> carIds)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Articles.AddAsync(article);
                await _context.SaveChangesAsync(); // Lưu để lấy ArticleId

                if (carIds != null && carIds.Any())
                {
                    var articleCars = carIds.Select(carId => new ArticleCar
                    {
                        ArticleId = article.ArticleId,
                        CarId = carId
                    });
                    await _context.ArticleCars.AddRangeAsync(articleCars);
                    await _context.SaveChangesAsync();
                }
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // 4. CẬP NHẬT (Logic đồng bộ hóa danh sách xe) 🚀
        public async Task UpdateArticleAsync(Article article, List<int> carIds)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Articles.Update(article);

                // --- Xử lý đồng bộ bảng trung gian ArticleCars ---
                // 1. Xóa hết các link xe cũ của bài viết này
                var existingLinks = _context.ArticleCars.Where(ac => ac.ArticleId == article.ArticleId);
                _context.ArticleCars.RemoveRange(existingLinks);

                // 2. Thêm mới các link xe từ danh sách carIds mới
                if (carIds != null && carIds.Any())
                {
                    var newLinks = carIds.Select(carId => new ArticleCar
                    {
                        ArticleId = article.ArticleId,
                        CarId = carId
                    });
                    await _context.ArticleCars.AddRangeAsync(newLinks);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // 5. XÓA BÀI VIẾT
        public async Task DeleteArticleAsync(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                // Cascade Delete đã được cấu hình trong Context nên 
                // khi xóa Article, các dòng liên quan trong ArticleCars sẽ tự mất theo.
                _context.Articles.Remove(article);
                await _context.SaveChangesAsync();
            }
        }
    }
}