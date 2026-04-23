namespace LogicBusiness.DTOs
{
    public class PricingVersionDto
    {
        public int PricingVersionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal PriceVnd { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class PricingCarDto
    {
        public int CarId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageKey { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public List<PricingVersionDto> Versions { get; set; } = new();
    }

    public class PricingCarBaseDto
    {
        public int CarId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class PricingVersionAdminDto
    {
        public int PricingVersionId { get; set; }
        public int CarId { get; set; }
        public string CarName { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string VersionName { get; set; } = string.Empty;
        public decimal PriceVnd { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class PricingVersionUpsertDto
    {
        public int CarId { get; set; }
        public string VersionName { get; set; } = string.Empty;
        public decimal PriceVnd { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
