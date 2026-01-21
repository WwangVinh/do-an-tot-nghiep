using CarSales.API.Models;

namespace CarSales.API.Repositories
{
    public interface IChatSessionRepository
    {
        Task<List<ChatSession>> GetAllAsync();
        Task<ChatSession?> GetByIdAsync(Guid id);
        Task AddAsync(ChatSession chatSession);
        Task UpdateAsync(ChatSession chatSession);
        Task DeleteAsync(ChatSession chatSession);
        bool Exists(Guid id);
        Task SaveAsync();
    }
}
