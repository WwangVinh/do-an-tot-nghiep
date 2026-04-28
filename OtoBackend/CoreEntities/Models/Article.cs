using LogicBusiness.DTOs;
using System;
using System.Collections.Generic;

namespace CoreEntities.Models;

public partial class Article
{
    public int ArticleId { get; set; }

    public string Title { get; set; } = null!;

    public string? Thumbnail { get; set; }

    public string Content { get; set; } = null!;

    public int AuthorId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool IsPublished { get; set; }

    public virtual User Author { get; set; } = null!;

    public virtual ICollection<ArticleCar> ArticleCars { get; set; } = new List<ArticleCar>();
}
