using CarSales.API.Models;

namespace CarSales.API.Services
{
    public interface IChatMessageService
    {
        Task<List<ChatMessage>> GetAllAsync();
        Task<ChatMessage?> GetByIdAsync(long id);
        Task<ChatMessage> CreateAsync(ChatMessage chatMessage);
        Task<bool> UpdateAsync(long id, ChatMessage chatMessage);
        Task<bool> DeleteAsync(long id);
    }
}
