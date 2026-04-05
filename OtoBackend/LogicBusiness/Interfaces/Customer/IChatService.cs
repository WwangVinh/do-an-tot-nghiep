using LogicBusiness.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Interfaces.Customer
{
    public interface IChatService
    {
        Task<IEnumerable<ChatMessageResponseDto>> GetChatHistoryAsync(int sessionId);
        Task<ChatMessageResponseDto?> SaveMessageAsync(SendMessageRequestDto request);

        Task<ChatSessionResponseDto?> GetCurrentSessionAsync(int? userId, string? guestToken);
        Task<ChatSessionResponseDto> CreateSessionAsync(int? userId, string? guestToken, int? assignedTo, int? showroomId);

        Task<IEnumerable<StaffForChatDto>> GetAvailableStaffAsync();
    }
}
