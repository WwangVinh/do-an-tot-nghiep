using CarSales.API.Models;
using CarSales.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarSales.API.Controllers      //Bảng Phiên chat AI (ChatSessions)
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatSessionsController : ControllerBase
    {
        private readonly IChatSessionService _service;

        public ChatSessionsController(IChatSessionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatSession>>> GetChatSessions()
        {
            return await _service.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ChatSession>> GetChatSession(Guid id)
        {
            var chatSession = await _service.GetByIdAsync(id);
            if (chatSession == null) return NotFound();
            return chatSession;
        }

        [HttpPost]
        public async Task<ActionResult<ChatSession>> PostChatSession(ChatSession chatSession)
        {
            var created = await _service.CreateAsync(chatSession);
            return CreatedAtAction(nameof(GetChatSession), new { id = created.SessionId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutChatSession(Guid id, ChatSession chatSession)
        {
            var result = await _service.UpdateAsync(id, chatSession);
            if (!result) return BadRequest();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChatSession(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
