using System;
using System.Collections.Generic;

namespace CarSales.API.Models;

public partial class ChatSession
{
    public Guid SessionId { get; set; }

    public int? UserId { get; set; }

    public string? Title { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    public virtual User? User { get; set; }
}
