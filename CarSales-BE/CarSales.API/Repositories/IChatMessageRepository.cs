using CarSales.API.Models;

namespace CarSales.API.Repositories
{
    public interface IChatMessageRepository
    {
        Task<List<ChatMessage>> GetAllAsync();
        Task<ChatMessage?> GetByIdAsync(long id);
        Task AddAsync(ChatMessage chatMessage);
        Task UpdateAsync(ChatMessage chatMessage);
        Task DeleteAsync(ChatMessage chatMessage);
        bool Exists(long id);
        Task SaveAsync();
    }
}
