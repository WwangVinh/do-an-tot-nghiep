using LogicBusiness.Interfaces.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace OtoBackend.Controllers.Shared
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notiService;

        public NotificationsController(INotificationService notiService) => _notiService = notiService;

        // FE gọi API này để lấy list thông báo (Nhớ truyền user/showroom ID từ token nhé, ở đây tui giả lập)
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications(int? userId, int? showroomId)
        {
            var data = await _notiService.GetMyNotificationsAsync(userId, showroomId);
            return Ok(new { Success = true, Data = data });
        }

        // FE gọi API này khi người dùng click vào cái chuông để tắt chấm đỏ
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result = await _notiService.MarkAsReadAsync(id);
            return Ok(new { Success = result });
        }
    }
}