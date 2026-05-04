using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.DTOs
{
    // DTO để nhận dữ liệu khi đăng bài (Ní đang bị thiếu cái này nè!)
    public class ArticleSubmitDto
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? Thumbnail { get; set; }
        public bool IsPublished { get; set; }
        public List<int> RelatedCarIds { get; set; } = new List<int>();
    }

    public class ArticleResponseDto
    {
        public int ArticleId { get; set; }
        public string Title { get; set; } = null!;
        public string? Thumbnail { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class ArticleDetailResponseDto
    {
        public int ArticleId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? Thumbnail { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsPublished { get; set; }
        public List<RelatedCarDto> RelatedCars { get; set; } = new List<RelatedCarDto>();
    }

    public class RelatedCarDto
    {
        public int CarId { get; set; }
        public string Name { get; set; } = null!;
        public decimal? Price { get; set; }
        public string? ImageUrl { get; set; }
    }
    public class ArticlePublicListResult
    {
        public IEnumerable<ArticleResponseDto> Data { get; set; } = new List<ArticleResponseDto>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
    }
}
