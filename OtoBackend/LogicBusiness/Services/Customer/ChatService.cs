using CoreEntities.Models;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Customer;
using LogicBusiness.Interfaces.Repositories;
using SqlServer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicBusiness.Services.Customer
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepo;
        private readonly IUserRepository _userRepo;

        public ChatService(IChatRepository chatRepo, IUserRepository userRepo)
        {
            _chatRepo = chatRepo;
            _userRepo = userRepo;
        }

        public async Task<IEnumerable<ChatMessageResponseDto>> GetChatHistoryAsync(int sessionId)
        {
            var messages = await _chatRepo.GetChatHistoryAsync(sessionId);
            return messages.Select(m => new ChatMessageResponseDto
            {
                MessageId = m.MessageId,
                SessionId = m.SessionId,
                SenderType = m.SenderType,
                MessageText = m.MessageText,
                CreatedAt = (DateTime)m.CreatedAt
            });
        }

        public async Task<ChatMessageResponseDto?> SaveMessageAsync(SendMessageRequestDto request)
        {
            var session = await _chatRepo.GetSessionByIdAsync(request.SessionId);
            if (session == null) return null; // Không tìm thấy phiên chat

            // 1. Tạo tin nhắn mới
            var newMessage = new ChatMessage
            {
                SessionId = request.SessionId,
                SenderType = request.SenderType,
                MessageText = request.MessageText,
                CreatedAt = DateTime.Now
                // IsRead mặc định là False (0)
            };
            await _chatRepo.AddMessageAsync(newMessage);

            // 2. Cập nhật thời gian hoạt động cuối cùng của Session
            session.LastMessageAt = DateTime.Now;
            await _chatRepo.UpdateSessionAsync(session);

            // 3. Trả về DTO để SignalR đẩy đi
            return new ChatMessageResponseDto
            {
                MessageId = newMessage.MessageId,
                SessionId = newMessage.SessionId,
                SenderType = newMessage.SenderType,
                MessageText = newMessage.MessageText,
                CreatedAt = (DateTime)newMessage.CreatedAt
            };
        }

        // Thêm 2 hàm xử lý này vào
        public async Task<ChatSessionResponseDto?> GetCurrentSessionAsync(int? userId, string? guestToken)
        {
            var session = await _chatRepo.GetActiveSessionAsync(userId, guestToken);

            if (session == null) return null;

            return new ChatSessionResponseDto
            {
                SessionId = session.SessionId,
                UserId = session.UserId,
                GuestToken = session.GuestToken,
                Status = session.Status
            };
        }

        public async Task<ChatSessionResponseDto> CreateSessionAsync(int? userId, string? guestToken, int? assignedTo)
        {
            var newSession = new ChatSession
            {
                UserId = userId,
                GuestToken = guestToken,
                AssignedTo = assignedTo,
                Status = "Open",
                CreatedAt = DateTime.Now,
                LastMessageAt = DateTime.Now
            };

            var savedSession = await _chatRepo.CreateSessionAsync(newSession);

            return new ChatSessionResponseDto
            {
                SessionId = savedSession.SessionId,
                UserId = savedSession.UserId,
                GuestToken = savedSession.GuestToken,
                Status = savedSession.Status
            };
        }

        public async Task<IEnumerable<StaffForChatDto>> GetAvailableStaffAsync()
        {
            var staffList = await _userRepo.GetStaffForChatAsync();

            return staffList.Select(s => new StaffForChatDto
            {
                UserId = s.UserId,
                FullName = s.FullName,
                Role = s.Role,
                ShowroomId = s.ShowroomId
            });
        }
    }
}
