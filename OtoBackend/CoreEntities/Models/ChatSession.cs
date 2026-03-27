using System;
using System.Collections.Generic;

namespace CoreEntities.Models;

public partial class ChatSession
{
    public int SessionId { get; set; }

    public int? UserId { get; set; }

    public string? GuestToken { get; set; }

    public int? AssignedTo { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? AssignedToNavigation { get; set; }

    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    public virtual User? User { get; set; }
    public DateTime LastMessageAt { get; set; }
}
