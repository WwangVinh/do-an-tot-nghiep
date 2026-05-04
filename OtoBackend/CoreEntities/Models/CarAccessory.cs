using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntities.Models
{
    [Table("CarAccessories")]
    public class CarAccessory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CarId { get; set; }

        [Required]
        public int AccessoryId { get; set; }

        [ForeignKey("CarId")]
        public virtual Car? Car { get; set; }

        [ForeignKey("AccessoryId")]
        public virtual Accessory? Accessory { get; set; }
    }
}
