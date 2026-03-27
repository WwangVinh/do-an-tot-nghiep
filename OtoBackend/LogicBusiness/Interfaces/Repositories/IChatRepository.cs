using CoreEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlServer.Repositories
{
    public interface IChatRepository
    {
        Task<ChatSession?> GetSessionByIdAsync(int sessionId);
        Task AddMessageAsync(ChatMessage message);
        Task UpdateSessionAsync(ChatSession session);
        Task<IEnumerable<ChatMessage>> GetChatHistoryAsync(int sessionId);

        // Thêm 2 dòng này vào dưới các hàm đã có
        Task<ChatSession?> GetActiveSessionAsync(int? userId, string? guestToken);
        Task<ChatSession> CreateSessionAsync(ChatSession session);
    }
}
