using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntities.Models
{
    public class CarFeature
    {
        public int CarId { get; set; }
        public int FeatureId { get; set; }

        public virtual Car Car { get; set; } = null!;
        public virtual Feature Feature { get; set; } = null!;
    }
}