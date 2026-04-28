using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntities.Models
{
    public class SystemSetting
    {
        [Key]
        [StringLength(50)]
        public string SettingKey { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string SettingValue { get; set; } = null!;

        public string? Description { get; set; }
    }
}
