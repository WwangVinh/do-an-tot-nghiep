using CarSales.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CarSales.API.Repositories
{
    public class ChatSessionRepository : IChatSessionRepository
    {
        private readonly CarSalesDbContext _context;

        public ChatSessionRepository(CarSalesDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChatSession>> GetAllAsync()
        {
            return await _context.ChatSessions.ToListAsync();
        }

        public async Task<ChatSession?> GetByIdAsync(Guid id)
        {
            return await _context.ChatSessions.FindAsync(id);
        }

        public async Task AddAsync(ChatSession chatSession)
        {
            _context.ChatSessions.Add(chatSession);
            await SaveAsync();
        }

        public async Task UpdateAsync(ChatSession chatSession)
        {
            _context.Entry(chatSession).State = EntityState.Modified;
            await SaveAsync();
        }

        public async Task DeleteAsync(ChatSession chatSession)
        {
            _context.ChatSessions.Remove(chatSession);
            await SaveAsync();
        }

        public bool Exists(Guid id)
        {
            return _context.ChatSessions.Any(e => e.SessionId == id);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
