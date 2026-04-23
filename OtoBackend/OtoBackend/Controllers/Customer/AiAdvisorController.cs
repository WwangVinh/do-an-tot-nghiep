using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OtoBackend.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class AiAdvisorController : ControllerBase
    {
        private readonly IAiAdvisorService _aiAdvisorService;

        public AiAdvisorController(IAiAdvisorService aiAdvisorService)
        {
            _aiAdvisorService = aiAdvisorService;
        }

        /// <summary>Tư vấn bằng AI; prompt có kèm snapshot danh sách xe từ DB.</summary>
        [HttpPost("chat")]
        [AllowAnonymous]
        public async Task<IActionResult> Chat([FromBody] AiAdvisorChatRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { message = "Nội dung tin nhắn không được để trống." });
            }

            try
            {
                var result = await _aiAdvisorService.GetReplyAsync(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(503, new { message = ex.Message });
            }
            catch (HttpRequestException ex)
            {
                var (status, message) = MapLlmClientError(ex.Message);
                return StatusCode(status, new { message, detail = ex.Message });
            }
        }

        /// <summary>Lỗi upstream (OpenAI/Gemini) qua HttpRequestException.</summary>
        private static (int status, string message) MapLlmClientError(string? detail)
        {
            var d = detail ?? "";

            if (d.Contains("Gemini HTTP", StringComparison.Ordinal))
            {
                if (d.Contains("API_KEY_INVALID", StringComparison.OrdinalIgnoreCase)
                    || d.Contains("PERMISSION_DENIED", StringComparison.OrdinalIgnoreCase)
                    || d.Contains("Gemini HTTP 403", StringComparison.Ordinal))
                {
                    return (502,
                        "Khóa Google AI (Gemini) không hợp lệ hoặc API chưa bật. Kiểm tra AiAdvisor:GeminiApiKey và bật Generative Language API trong Google Cloud.");
                }

                if (d.Contains("RESOURCE_EXHAUSTED", StringComparison.OrdinalIgnoreCase)
                    || d.Contains("Gemini HTTP 429", StringComparison.Ordinal))
                {
                    if (d.Contains("free_tier", StringComparison.OrdinalIgnoreCase)
                        && d.Contains("limit: 0", StringComparison.Ordinal))
                    {
                        return (429,
                            "Gói miễn phí không còn quota cho model đang dùng (Google báo limit: 0). Thử AiAdvisor:Model = gemini-2.5-flash-lite, bật billing, hoặc đợi rate limit. Chi tiết: https://ai.google.dev/gemini-api/docs/rate-limits");
                    }

                    return (429,
                        "Gemini/Google AI báo hết quota hoặc vượt giới hạn. Xem https://ai.google.dev/gemini-api/docs/rate-limits và https://ai.dev/rate-limit . Có thể cần bật thanh toán hoặc đổi model (ví dụ gemini-2.5-flash-lite).");
                }

                if (d.Contains("NOT_FOUND", StringComparison.OrdinalIgnoreCase)
                    || d.Contains("Gemini HTTP 404", StringComparison.Ordinal)
                    || d.Contains("is not found", StringComparison.OrdinalIgnoreCase)
                    || d.Contains("was not found", StringComparison.OrdinalIgnoreCase))
                {
                    return (502,
                        "Model Gemini không tồn tại hoặc đã ngừng (thường gặp với gemini-1.5-*). Đổi AiAdvisor:Model sang gemini-2.5-flash hoặc gemini-2.5-flash-lite. Xem https://ai.google.dev/gemini-api/docs/models");
                }

                if (d.Contains("Unknown name \"systemInstruction\"", StringComparison.Ordinal)
                    || d.Contains("unknown name \"systeminstruction\"", StringComparison.OrdinalIgnoreCase))
                {
                    return (502,
                        "Endpoint Gemini /v1 không hỗ trợ systemInstruction. Đặt AiAdvisor:GeminiApiVersion thành v1beta (mặc định) hoặc để backend tự nhúng system vào contents.");
                }

                if (d.Contains("INVALID_ARGUMENT", StringComparison.OrdinalIgnoreCase)
                    || d.Contains("Gemini HTTP 400", StringComparison.Ordinal))
                {
                    return (502,
                        "Gemini từ chối yêu cầu (400). Thử giảm AiAdvisor:MaxCatalogCars hoặc kiểm tra nội dung chat; xem JSON trong trường detail của response API.");
                }

                if (d.Contains("UNAUTHENTICATED", StringComparison.OrdinalIgnoreCase)
                    || d.Contains("Gemini HTTP 401", StringComparison.Ordinal))
                {
                    return (502,
                        "Khóa API Gemini không được chấp nhận (401). Tạo lại key trên Google AI Studio và cập nhật AiAdvisor:GeminiApiKey.");
                }

                return (502,
                    "Gemini trả lỗi. Kiểm tra AiAdvisor:Model, GeminiApiKey và log/response detail. Danh sách model: https://ai.google.dev/gemini-api/docs/models");
            }

            if (d.Contains("insufficient_quota", StringComparison.OrdinalIgnoreCase))
            {
                return (503,
                    "Tài khoản OpenAI đã hết quota hoặc chưa cấu hình thanh toán. Vào https://platform.openai.com/account/billing để nạp credit hoặc nâng gói, rồi thử lại.");
            }

            if (d.Contains("invalid_api_key", StringComparison.OrdinalIgnoreCase)
                || d.Contains("OpenAI HTTP 401", StringComparison.Ordinal))
            {
                return (502, "Khóa API OpenAI không hợp lệ hoặc đã bị thu hồi. Kiểm tra AiAdvisor:OpenAIApiKey trong cấu hình.");
            }

            if (d.Contains("rate_limit", StringComparison.OrdinalIgnoreCase))
            {
                return (429, "Nhà cung cấp AI đang giới hạn tốc độ gọi API. Đợi vài giây rồi thử lại.");
            }

            if (d.Contains("OpenAI HTTP 429", StringComparison.Ordinal))
            {
                return (429, "OpenAI từ chối yêu cầu (429). Xem phần detail hoặc https://platform.openai.com/account/billing (quota/thanh toán).");
            }

            return (502, "Không gọi được dịch vụ AI. Kiểm tra AiAdvisor (Provider, khóa API, quota) và kết nối mạng.");
        }
    }
}
