using System;
using System.Collections.Generic;

namespace OtoBackend.Models;

public partial class LocationTaxis
{
    public int LocationId { get; set; }

    public string CityName { get; set; } = null!;

    public decimal RegistrationTaxPercent { get; set; }

    public decimal LicensePlateFee { get; set; }
}
