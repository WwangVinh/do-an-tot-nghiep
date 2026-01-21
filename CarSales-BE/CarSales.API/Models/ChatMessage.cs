using System;
using System.Collections.Generic;

namespace CarSales.API.Models;

public partial class ChatMessage
{
    public long MessageId { get; set; }

    public Guid SessionId { get; set; }

    public bool IsUserMessage { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? Timestamp { get; set; }

    public virtual ChatSession Session { get; set; } = null!;
}
