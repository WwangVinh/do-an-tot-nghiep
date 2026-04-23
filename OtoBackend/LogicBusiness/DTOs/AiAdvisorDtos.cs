using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LogicBusiness.DTOs
{
    public class AiChatTurnDto
    {
        /// <summary>user hoặc assistant</summary>
        public string Role { get; set; } = "";

        public string Content { get; set; } = "";
    }

    public class AiAdvisorChatRequestDto
    {
        [Required]
        [MinLength(1)]
        public string Message { get; set; } = "";

        /// <summary>Các lượt trước (tối đa ~20 lượt phía server).</summary>
        public List<AiChatTurnDto>? History { get; set; }
    }

    public class AiAdvisorChatResponseDto
    {
        public string Reply { get; set; } = "";

        /// <summary>carId từ JSON catalog; FE có thể gọi GET /api/Cars/{id} để hiển thị thẻ.</summary>
        public List<int>? SuggestedCarIds { get; set; }
    }
}
