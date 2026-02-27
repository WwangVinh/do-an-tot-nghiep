using System;
using System.Collections.Generic;

namespace OtoBackend.Models;

public partial class SystemAuditLog
{
    public int LogId { get; set; }

    public int UserId { get; set; }

    public string ActionType { get; set; } = null!;

    public string? TableName { get; set; }

    public string? RecordId { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
