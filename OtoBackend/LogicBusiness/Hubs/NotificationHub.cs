using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogicBusiness.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            // Dùng Token để nhận diện xem ai đang đăng nhập
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
            var showroomId = Context.User?.FindFirst("ShowroomId")?.Value;

            // Gom chung 1 nhóm
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
            }
            if (!string.IsNullOrEmpty(role))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Role_{role}");
            }
            if (!string.IsNullOrEmpty(showroomId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Showroom_{showroomId}");

                if (!string.IsNullOrEmpty(role))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"Showroom_{showroomId}_Role_{role}");
                }
            }

            await base.OnConnectedAsync();
        }
    }
}


