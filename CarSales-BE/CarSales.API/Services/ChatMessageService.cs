using CarSales.API.Models;
using CarSales.API.Repositories;

namespace CarSales.API.Services
{
    public class ChatMessageService : IChatMessageService
    {
        private readonly IChatMessageRepository _repository;

        public ChatMessageService(IChatMessageRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ChatMessage>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ChatMessage?> GetByIdAsync(long id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<ChatMessage> CreateAsync(ChatMessage chatMessage)
        {
            await _repository.AddAsync(chatMessage);
            return chatMessage;
        }

        public async Task<bool> UpdateAsync(long id, ChatMessage chatMessage)
        {
            if (id != chatMessage.MessageId || !_repository.Exists(id))
                return false;

            await _repository.UpdateAsync(chatMessage);
            return true;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var chatMessage = await _repository.GetByIdAsync(id);
            if (chatMessage == null) return false;

            await _repository.DeleteAsync(chatMessage);
            return true;
        }
    }
}
