using System;
using System.Collections.Generic;

namespace OtoBackend.Models;

public partial class UserActivity
{
    public int ActivityId { get; set; }

    public int? UserId { get; set; }

    public string? ActivityType { get; set; }

    public int? CarId { get; set; }

    public DateTime? ActivityDate { get; set; }

    public virtual Car? Car { get; set; }

    public virtual User? User { get; set; }
}
