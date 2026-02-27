using System;
using System.Collections.Generic;

namespace OtoBackend.Models;

public partial class Banner
{
    public int BannerId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? LinkUrl { get; set; }

    public int Position { get; set; }

    public bool IsActive { get; set; }

    public string BannerName { get; set; } = null!;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
