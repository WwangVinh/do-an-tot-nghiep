using System;
using System.Collections.Generic;

namespace OtoBackend.Models;

public partial class CarImage
{
    public int CarImageId { get; set; }

    public int? CarId { get; set; }

    public string? ImageUrl { get; set; }

    public bool? IsMainImage { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool Is360Degree { get; set; }

    public virtual Car? Car { get; set; }
}
