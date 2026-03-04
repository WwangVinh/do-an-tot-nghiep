using System;
using System.Collections.Generic;

namespace CoreEntities.Models;

public partial class CarImage
{
    public int CarImageId { get; set; }

    public int? CarId { get; set; }

    public string? ImageUrl { get; set; }

    public bool? IsMainImage { get; set; }

    public string? ImageType { get; set; } // Phân loại: Nội thất, Ngoại thất...

    public string? FileHash { get; set; }  // Mã vân tay của file để chống trùng

    public DateTime? CreatedAt { get; set; }

    public bool Is360Degree { get; set; }

    public string? Description { get; set; }

    public string? Title { get; set; }

    public virtual Car? Car { get; set; }
}
