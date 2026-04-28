using CoreEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Repositories
{
    public interface IChatRepository
    {
        Task<ChatSession?> GetSessionByIdAsync(int sessionId);
        Task AddMessageAsync(ChatMessage message);
        Task UpdateSessionAsync(ChatSession session);
        Task<IEnumerable<ChatMessage>> GetChatHistoryAsync(int sessionId);

        Task<ChatSession?> GetActiveSessionAsync(int? userId, string? guestToken);
        Task<ChatSession> CreateSessionAsync(ChatSession session);
    }
}
