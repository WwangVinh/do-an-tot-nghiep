using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreEntities.Models
{
    public partial class Accessory
    {
        [Key]
        public int AccessoryId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Danh sách xe nào được gán phụ kiện này
        public virtual ICollection<CarAccessory> CarAccessories { get; set; } = new List<CarAccessory>();
    }
}