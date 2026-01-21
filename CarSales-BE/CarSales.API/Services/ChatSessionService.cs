using CarSales.API.Models;
using CarSales.API.Repositories;

namespace CarSales.API.Services
{
    public class ChatSessionService : IChatSessionService
    {
        private readonly IChatSessionRepository _repository;

        public ChatSessionService(IChatSessionRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ChatSession>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ChatSession?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<ChatSession> CreateAsync(ChatSession chatSession)
        {
            await _repository.AddAsync(chatSession);
            return chatSession;
        }

        public async Task<bool> UpdateAsync(Guid id, ChatSession chatSession)
        {
            if (id != chatSession.SessionId || !_repository.Exists(id))
                return false;

            await _repository.UpdateAsync(chatSession);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var chatSession = await _repository.GetByIdAsync(id);
            if (chatSession == null) return false;

            await _repository.DeleteAsync(chatSession);
            return true;
        }
    }
}
