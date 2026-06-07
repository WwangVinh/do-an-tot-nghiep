using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Customer;
using LogicBusiness.Interfaces.Shared;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace LogicBusiness.Services.Customer
{
    public class AiAdvisorService : IAiAdvisorService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly ICarService _carService;
        private readonly IShowroomService _showroomService;
        private readonly IArticleService _articleService;
        private readonly IPricingAdminService _pricingAdminService;
        private readonly INotificationService _notificationService;

        public AiAdvisorService(
            HttpClient http,
            IConfiguration config,
            ICarService carService,
            IShowroomService showroomService,
            IArticleService articleService,
            IPricingAdminService pricingAdminService,
            INotificationService notificationService)
        {
            _http = http;
            _config = config;
            _carService = carService;
            _showroomService = showroomService;
            _articleService = articleService;
            _pricingAdminService = pricingAdminService;
            _notificationService = notificationService;
        }

        public async Task<AiAdvisorChatResponseDto> GetReplyAsync(AiAdvisorChatRequestDto request)
        {
            // Detect Vietnamese phone number
            var phoneMatch = Regex.Match(request.Message, @"\b0[35789]\d{8}\b");

            if (phoneMatch.Success)
            {
                var phone = phoneMatch.Value;

                await _notificationService.CreateNotificationAsync(
                    userId: null,
                    showroomId: null,
                    roleTarget: AppRoles.Sales,
                    title: "Khách hàng để lại SĐT qua AI Chat",
                    content: $"SĐT: {phone}. Khách nhắn: \"{request.Message}\".",
                    actionUrl: "/admin/leads",
                    type: "LEAD"
                );
            }

            var provider = (_config["AiAdvisor:Provider"] ?? "OpenAI").Trim();

            if (!provider.Equals("Gemini", StringComparison.OrdinalIgnoreCase)
                && !provider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"AiAdvisor:Provider không hợp lệ ({provider}).");
            }

            var maxCars = Math.Clamp(
                int.TryParse(_config["AiAdvisor:MaxCatalogCars"], out var n) ? n : 60,
                1,
                100);

            var knowledgeBaseJson = await BuildKnowledgeBaseAsync(maxCars);

            // ==============================
            // SUPER SALES SYSTEM PROMPT
            // ==============================

            var systemPrompt =
                "Ban la mot chuyen vien tu van ban xe o to cao cap chuyen nghiep tai showroom.\n" +
                "Ban noi chuyen tu nhien, lich su, than thien, khong may moc.\n" +
                "Muc tieu cua ban la tu van dung nhu cau va tao trai nghiem chuyen nghiep nhu nhan vien sales that.\n\n" +

                "NGUYEN TAC TU VAN:\n" +
                "- Luon phan tich nhu cau truoc khi de xuat xe.\n" +
                "- Hoi va suy luan theo: ngan sach, muc dich su dung, so nguoi di, thich tiet kiem hay manh me.\n" +
                "- Neu khach mua lan dau, giai thich de hieu.\n" +
                "- Neu khach hieu ve xe, tu van sau hon nhu mot chuyen gia.\n" +
                "- Neu khach phan van nhieu xe, hay so sanh ngan gon.\n" +
                "- Neu co khuyen mai, nhan manh loi ich tai chinh.\n" +
                "- Neu xe phu hop gia dinh, hay nhan manh rong rai, an toan, tiet kiem.\n" +
                "- Neu xe phu hop nguoi tre, hay nhan manh cong nghe, the thao, trai nghiem lai.\n" +
                "- Luon uu tien trai nghiem khach hang thay vi ep mua.\n" +
                "- Khuyen khich khach dat lich lai thu neu phu hop.\n" +
                "- Co kha nang nho so thich cua khach trong suot cuoc hoi thoai.\n\n" +

                "CAC TINH NANG WEBSITE:\n" +
                "- Dat lich lai thu.\n" +
                "- Them vao gio hang.\n" +
                "- Ky gui xe cu.\n" +
                "- Xem danh gia.\n" +
                "- Xem showroom.\n" +
                "- Xem khuyen mai.\n\n" +

                "LUU Y QUAN TRONG:\n" +
                "1. Khong duoc tu bia thong tin.\n" +
                "2. Chi dung du lieu trong JSON.\n" +
                "3. Neu khong co thong tin, hay noi ro va moi khach de lai lien he.\n" +
                "4. Neu nhac toi bat ky xe nao thi BAT BUOC phai tra ve [CAR_IDS:id]\n" +
                "5. Dat [CAR_IDS] o CUOI cau tra loi.\n" +
                "6. Toi da 8 xe.\n" +
                "7. Khong duoc quen [CAR_IDS].\n\n" +

                "QUY TAC PHIEN BAN:\n" +
                "- Tim carId trong Cars.\n" +
                "- Sau do doi chieu CarVersions theo carId.\n\n" +

                "DU LIEU JSON:\n" +
                knowledgeBaseJson;

            // ==============================
            // PROVIDER
            // ==============================

            if (provider.Equals("Gemini", StringComparison.OrdinalIgnoreCase))
            {
                var geminiKey = _config["AiAdvisor:GeminiApiKey"];

                if (string.IsNullOrWhiteSpace(geminiKey))
                {
                    throw new InvalidOperationException("Thiếu Gemini API Key.");
                }

                var model = ResolveGeminiModel(_config["AiAdvisor:Model"]);

                var geminiReply = await GetReplyGeminiAsync(
                    request,
                    systemPrompt,
                    geminiKey.Trim(),
                    model);

                return ApplySuggestedCarIds(geminiReply);
            }

            var openAiReply = await GetReplyOpenAiAsync(request, systemPrompt);

            return ApplySuggestedCarIds(openAiReply);
        }

        // ==========================================
        // APPLY CAR IDS
        // ==========================================

        private static AiAdvisorChatResponseDto ApplySuggestedCarIds(AiAdvisorChatResponseDto dto)
        {
            var (text, ids) = ExtractSuggestedCarIds(dto.Reply);

            dto.Reply = text;
            dto.SuggestedCarIds = ids;

            return dto;
        }

        private static (string reply, List<int>? ids) ExtractSuggestedCarIds(string reply)
        {
            if (string.IsNullOrWhiteSpace(reply))
                return (reply, null);

            var ids = new List<int>();

            // NEW FORMAT
            var match = Regex.Match(
                reply,
                @"\[CAR_IDS:\s*([\d,\s]+)\s*\]",
                RegexOptions.CultureInvariant);

            if (match.Success)
            {
                var idStrings = match.Groups[1]
                    .Value
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                foreach (var part in idStrings)
                {
                    if (int.TryParse(part, out var id) && id > 0)
                    {
                        ids.Add(id);
                    }
                }
            }

            // OLD FORMAT SUPPORT
            if (!match.Success)
            {
                match = Regex.Match(
                    reply,
                    @"\[CAR_ID:\s*(\d+)\s*\]",
                    RegexOptions.CultureInvariant);

                if (match.Success)
                {
                    if (int.TryParse(match.Groups[1].Value, out var singleId))
                    {
                        ids.Add(singleId);
                    }
                }
            }

            ids = ids.Distinct().Take(8).ToList();

            // CLEAN TAGS
            var cleaned = Regex.Replace(
                reply,
                @"\[CAR_IDS:\s*[\d,\s]+\s*\]",
                "",
                RegexOptions.CultureInvariant);

            cleaned = Regex.Replace(
                cleaned,
                @"\[CAR_ID:\s*\d+\s*\]",
                "",
                RegexOptions.CultureInvariant);

            cleaned = cleaned.Trim();

            return (cleaned, ids.Count > 0 ? ids : null);
        }

        // ==========================================
        // MODEL RESOLVER
        // ==========================================

        private static string ResolveGeminiModel(string? configured)
        {
            var m = (configured ?? "").Trim();

            if (string.IsNullOrWhiteSpace(m))
                return "gemini-2.5-flash";

            if (m.StartsWith("gpt-", StringComparison.OrdinalIgnoreCase))
                return "gemini-2.5-flash";

            return m;
        }

        // ==========================================
        // OPENAI
        // ==========================================

        private async Task<AiAdvisorChatResponseDto> GetReplyOpenAiAsync(
            AiAdvisorChatRequestDto request,
            string systemPrompt)
        {
            var apiKey = _config["AiAdvisor:OpenAIApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("Thiếu OpenAI API Key.");
            }

            var model = _config["AiAdvisor:Model"] ?? "gpt-4o-mini";

            var baseUrl = (_config["AiAdvisor:BaseUrl"]
                ?? "https://api.openai.com/v1")
                .TrimEnd('/');

            var messages = BuildOpenAiMessages(request, systemPrompt);

            var payload = new Dictionary<string, object?>
            {
                ["model"] = model,
                ["messages"] = messages,
                ["temperature"] = 0.55
            };

            var req = new HttpRequestMessage(
                HttpMethod.Post,
                $"{baseUrl}/chat/completions")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json")
            };

            req.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey.Trim());

            _http.Timeout = TimeSpan.FromSeconds(90);

            var response = await _http.SendAsync(req);

            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"OpenAI HTTP {(int)response.StatusCode}: {responseText}");
            }

            using var doc = JsonDocument.Parse(responseText);

            var reply = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "";

            return new AiAdvisorChatResponseDto
            {
                Reply = reply
            };
        }

        // ==========================================
        // GEMINI
        // ==========================================

        private async Task<AiAdvisorChatResponseDto> GetReplyGeminiAsync(
            AiAdvisorChatRequestDto request,
            string systemPrompt,
            string apiKey,
            string model)
        {
            var contents = BuildGeminiContents(request);

            var payload = new Dictionary<string, object?>
            {
                ["systemInstruction"] = new Dictionary<string, object?>
                {
                    ["parts"] = new object[]
                    {
                        new Dictionary<string, string>
                        {
                            ["text"] = systemPrompt
                        }
                    }
                },
                ["contents"] = contents,
                ["generationConfig"] = new Dictionary<string, object?>
                {
                    ["temperature"] = 0.55d
                }
            };

            var json = JsonSerializer.Serialize(payload);

            var url =
                $"https://generativelanguage.googleapis.com/v1beta/models/" +
                $"{Uri.EscapeDataString(model)}:generateContent" +
                $"?key={Uri.EscapeDataString(apiKey)}";

            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json")
            };

            _http.Timeout = TimeSpan.FromSeconds(90);

            var response = await _http.SendAsync(req);

            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Gemini HTTP {(int)response.StatusCode}: {responseText}");
            }

            using var doc = JsonDocument.Parse(responseText);

            var candidates = doc.RootElement.GetProperty("candidates");

            var parts = candidates[0]
                .GetProperty("content")
                .GetProperty("parts");

            var sb = new StringBuilder();

            foreach (var part in parts.EnumerateArray())
            {
                if (part.TryGetProperty("text", out var t))
                {
                    sb.Append(t.GetString());
                }
            }

            return new AiAdvisorChatResponseDto
            {
                Reply = sb.ToString()
            };
        }

        // ==========================================
        // BUILD MESSAGES
        // ==========================================

        private static List<Dictionary<string, object?>> BuildGeminiContents(
            AiAdvisorChatRequestDto request)
        {
            var list = new List<Dictionary<string, object?>>();

            if (request.History is { Count: > 0 })
            {
                foreach (var turn in request.History.TakeLast(12))
                {
                    var role =
                        string.Equals(turn.Role, "assistant", StringComparison.OrdinalIgnoreCase)
                        ? "model"
                        : "user";

                    list.Add(new Dictionary<string, object?>
                    {
                        ["role"] = role,
                        ["parts"] = new object[]
                        {
                            new Dictionary<string, string>
                            {
                                ["text"] = turn.Content ?? ""
                            }
                        }
                    });
                }
            }

            list.Add(new Dictionary<string, object?>
            {
                ["role"] = "user",
                ["parts"] = new object[]
                {
                    new Dictionary<string, string>
                    {
                        ["text"] = request.Message.Trim()
                    }
                }
            });

            return list;
        }

        private static List<Dictionary<string, string>> BuildOpenAiMessages(
            AiAdvisorChatRequestDto request,
            string systemPrompt)
        {
            var messages = new List<Dictionary<string, string>>
            {
                new()
                {
                    ["role"] = "system",
                    ["content"] = systemPrompt
                }
            };

            if (request.History is { Count: > 0 })
            {
                foreach (var turn in request.History.TakeLast(12))
                {
                    messages.Add(new Dictionary<string, string>
                    {
                        ["role"] =
                            turn.Role?.ToLowerInvariant() == "assistant"
                            ? "assistant"
                            : "user",

                        ["content"] = turn.Content ?? ""
                    });
                }
            }

            messages.Add(new Dictionary<string, string>
            {
                ["role"] = "user",
                ["content"] = request.Message.Trim()
            });

            return messages;
        }

        // ==========================================
        // BUILD KNOWLEDGE BASE
        // ==========================================

        private async Task<string> BuildKnowledgeBaseAsync(int maxCars)
        {
            object? cars = null;
            object? showrooms = null;
            object? articles = null;
            object? carVersions = null;

            try
            {
                cars = await _carService.GetCarsAsync(
                    search: null,
                    brand: null,
                    color: null,
                    minPrice: null,
                    maxPrice: null,
                    status: null,
                    transmission: null,
                    bodyStyle: null,
                    fuelType: null,
                    location: null,
                    condition: null,
                    minYear: null,
                    maxYear: null,
                    sort: null,
                    inStockOnly: false,
                    page: 1,
                    pageSize: maxCars);
            }
            catch { }

            try
            {
                var rawShowrooms =
                    await _showroomService.GetAllShowroomsAsync();

                showrooms = rawShowrooms.Select(s => new
                {
                    s.ShowroomId,
                    s.Name,
                    s.Hotline,
                    s.Province,
                    s.District,
                    s.StreetAddress
                });
            }
            catch { }

            try
            {
                articles =
                    await _articleService.GetArticlesAdminAsync(
                        null,
                        1,
                        5,
                        true);
            }
            catch { }

            try
            {
                var rawVersions =
                    await _pricingAdminService.GetAllAsync(null, true);

                carVersions = rawVersions.Select(v => new
                {
                    v.CarId,
                    v.VersionName,
                    v.PriceVnd
                });
            }
            catch { }

            var finalKnowledge = new
            {
                Cars = cars,
                CarVersions = carVersions,
                Showrooms = showrooms,
                ArticlesAndEvents = articles
            };

            return JsonSerializer.Serialize(
                finalKnowledge,
                new JsonSerializerOptions
                {
                    WriteIndented = false,
                    ReferenceHandler =
                        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
                });
        }
    }
}