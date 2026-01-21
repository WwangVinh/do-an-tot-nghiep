using CarSales.API.Models;
using CarSales.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarSales.API.Controllers      //Bảng Tin nhắn Chat (ChatMessages)
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatMessagesController : ControllerBase
    {
        private readonly IChatMessageService _service;

        public ChatMessagesController(IChatMessageService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatMessage>>> GetChatMessages()
        {
            return await _service.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ChatMessage>> GetChatMessage(long id)
        {
            var chatMessage = await _service.GetByIdAsync(id);
            if (chatMessage == null) return NotFound();
            return chatMessage;
        }

        [HttpPost]
        public async Task<ActionResult<ChatMessage>> PostChatMessage(ChatMessage chatMessage)
        {
            var created = await _service.CreateAsync(chatMessage);
            return CreatedAtAction(nameof(GetChatMessage), new { id = created.MessageId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutChatMessage(long id, ChatMessage chatMessage)
        {
            var result = await _service.UpdateAsync(id, chatMessage);
            if (!result) return BadRequest();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChatMessage(long id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
