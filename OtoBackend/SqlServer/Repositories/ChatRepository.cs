using CoreEntities.Models;
using LogicBusiness.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlServer.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly OtoContext _context;

        public ChatRepository(OtoContext context)
        {
            _context = context;
        }

        public async Task<ChatSession?> GetSessionByIdAsync(int sessionId)
        {
            return await _context.ChatSessions.FindAsync(sessionId);
        }

        public async Task AddMessageAsync(ChatMessage message)
        {
            await _context.ChatMessages.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSessionAsync(ChatSession session)
        {
            _context.ChatSessions.Update(session);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ChatMessage>> GetChatHistoryAsync(int sessionId)
        {
            // Lấy toàn bộ tin nhắn của 1 phiên chat, sắp xếp cũ nhất lên trước
            return await _context.ChatMessages
                .Where(m => m.SessionId == sessionId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        // Thêm 2 hàm này vào để thực thi interface
        public async Task<ChatSession?> GetActiveSessionAsync(int? userId, string? guestToken)
        {
            // Chỉ lấy những phiên chat đang mở (chưa bị đóng)
            var query = _context.ChatSessions.Where(s => s.Status == "Open");

            // Ưu tiên check theo UserId nếu khách đã đăng nhập
            if (userId.HasValue)
            {
                query = query.Where(s => s.UserId == userId.Value);
            }
            // Nếu chưa đăng nhập, check theo GuestToken mà React sinh ra
            else if (!string.IsNullOrEmpty(guestToken))
            {
                query = query.Where(s => s.GuestToken == guestToken);
            }
            else
            {
                return null;
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<ChatSession> CreateSessionAsync(ChatSession session)
        {
            await _context.ChatSessions.AddAsync(session);
            await _context.SaveChangesAsync();
            return session; // Sau khi SaveChanges, EF Core sẽ tự động điền ID mới vào object này
        }
    }
}
