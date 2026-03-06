using System;
using System.Collections.Generic;

namespace CoreEntities.Models;

public partial class Feature
{
    public int FeatureId { get; set; }

    public string FeatureName { get; set; } = null!;

    public string? Icon { get; set; }

    //public virtual ICollection<Car> Cars { get; set; } = new List<Car>();

    public virtual ICollection<CarFeature> CarFeatures { get; set; } = new List<CarFeature>();
}
