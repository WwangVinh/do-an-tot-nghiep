using CarSales.API.Models;

namespace CarSales.API.Services
{
    public interface IChatSessionService
    {
        Task<List<ChatSession>> GetAllAsync();
        Task<ChatSession?> GetByIdAsync(Guid id);
        Task<ChatSession> CreateAsync(ChatSession chatSession);
        Task<bool> UpdateAsync(Guid id, ChatSession chatSession);
        Task<bool> DeleteAsync(Guid id);
    }
}
