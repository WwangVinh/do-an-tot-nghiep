using CoreEntities.Models;
using System;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntities.Models;

[Table("CarInventories")]
public class CarInventory
{
    [Key]
    public int InventoryId { get; set; }

    [Required]
    public int ShowroomId { get; set; }

    [Required]
    public int CarId { get; set; }

    [Required]
    public int Quantity { get; set; } = 0;

    [Required]
    [MaxLength(50)]
    public string DisplayStatus { get; set; } = "OnDisplay";

    public DateTime? UpdatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    [ForeignKey("ShowroomId")]
    public virtual Showroom? Showroom { get; set; }

    [ForeignKey("CarId")]
    public virtual Car? Car { get; set; }
}