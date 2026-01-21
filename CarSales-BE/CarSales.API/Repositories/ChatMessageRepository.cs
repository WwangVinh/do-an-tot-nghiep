using CarSales.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CarSales.API.Repositories
{
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly CarSalesDbContext _context;

        public ChatMessageRepository(CarSalesDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChatMessage>> GetAllAsync()
        {
            return await _context.ChatMessages.ToListAsync();
        }

        public async Task<ChatMessage?> GetByIdAsync(long id)
        {
            return await _context.ChatMessages.FindAsync(id);
        }

        public async Task AddAsync(ChatMessage chatMessage)
        {
            _context.ChatMessages.Add(chatMessage);
            await SaveAsync();
        }

        public async Task UpdateAsync(ChatMessage chatMessage)
        {
            _context.Entry(chatMessage).State = EntityState.Modified;
            await SaveAsync();
        }

        public async Task DeleteAsync(ChatMessage chatMessage)
        {
            _context.ChatMessages.Remove(chatMessage);
            await SaveAsync();
        }

        public bool Exists(long id)
        {
            return _context.ChatMessages.Any(e => e.MessageId == id);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
