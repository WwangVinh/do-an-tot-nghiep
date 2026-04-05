using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace LogicBusiness.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        // Khách hàng hoặc Admin tham gia vào 1 "Phòng Chat" (Dựa theo SessionId)
        public async Task JoinChatSession(int sessionId)
        {
            string groupName = $"Session_{sessionId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        // Hàm nhận tin nhắn từ React, lưu DB và bắn ngược lại cho những người trong phòng
        public async Task SendMessage(SendMessageRequestDto request)
        {
            // Lưu xuống SQL Database
            var savedMessage = await _chatService.SaveMessageAsync(request);

            if (savedMessage != null)
            {
                string groupName = $"Session_{request.SessionId}";

                // Phát sóng tin nhắn này cho tất cả ai đang ở trong phòng chat này
                // React sẽ lắng nghe sự kiện "ReceiveMessage" này
                await Clients.Group(groupName).SendAsync("ReceiveMessage", savedMessage);
            }
        }
    }
}
