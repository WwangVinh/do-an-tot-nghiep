using System;
using System.Collections.Generic;

namespace OtoBackend.Models;

public partial class UserLogin
{
    public string LoginProvider { get; set; } = null!;

    public string ProviderKey { get; set; } = null!;

    public int UserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
