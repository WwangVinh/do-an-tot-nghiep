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
            // Phát hiện số điện thoại Việt Nam trong tin nhắn
            var phoneMatch = Regex.Match(request.Message, @"\b0[35789]\d{8}\b");

            if (phoneMatch.Success)
            {
                var phone = phoneMatch.Value;

                // Kích hoạt Notification bắn ngay cho team Sales
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

            // Validate provider
            var provider = (_config["AiAdvisor:Provider"] ?? "OpenAI").Trim();
            if (!provider.Equals("Gemini", StringComparison.OrdinalIgnoreCase)
                && !provider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase))
            {
                if (provider.StartsWith("AIza", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "AiAdvisor:Provider phai la chu Gemini hoac OpenAI, khong phai API key. " +
                        "Dat khoa Google vao AiAdvisor:GeminiApiKey va \"Provider\": \"Gemini\".");
                }

                throw new InvalidOperationException(
                    $"AiAdvisor:Provider khong hop le ({provider}). Chi dung \"Gemini\" hoac \"OpenAI\".");
            }

            var maxCars = Math.Clamp(
                int.TryParse(_config["AiAdvisor:MaxCatalogCars"], out var n) ? n : 60,
                1, 100);

            // Xây dựng KnowledgeBase JSON từ DB
            var knowledgeBaseJson = await BuildKnowledgeBaseAsync(maxCars);

            // System prompt với ràng buộc nghiệp vụ và quy ước AI_CAR_IDS
            var systemPrompt =
                "Ban la tro ly tu van tai showroom o to, tra loi bang tieng Viet.\n" +
                "Chi dua tren thong tin trong khoi JSON duoi day khi tu van ve: " +
                "danh sach xe, gia ca, phien ban, thong tin ton kho, he thong showroom, " +
                "va cac bai viet/su kien moi nhat.\n" +
                "LUU Y CUC KY QUAN TRONG: De biet xe co nhung phien ban nao, hay tim 'carId' " +
                "cua xe do trong mang 'Cars', sau do tim tat ca cac phien ban co cung 'carId' " +
                "nam trong mang 'CarVersions'.\n" +
                "Neu khong co thong tin trong du lieu, hay noi ro ban khong nam thong tin do " +
                "va moi khach goi hotline hoac de lai lien he.\n" +
                "Khong duoc tu bia so lieu, dia chi hay su kien. " +
                "Giu giong dieu than thien, ro rang, chuyen nghiep.\n\n" +
                "DU LIEU HE THONG (JSON chua Cars, CarVersions, Showrooms, Articles):\n" +
                knowledgeBaseJson +
                "\n\nQUY UOC MAY DOC (bat buoc khi liet ke xe cu the): " +
                "Moi xe trong JSON co truong carId. " +
                "Khi ban de xuat hoac liet ke cac xe cu the tu JSON, " +
                "them DUNG mot dong tuyet doi CUOI CUNG cua tin nhan, " +
                "khong co ky tu nao sau do, dinh dang: <!--AI_CAR_IDS:id1,id2--> " +
                "voi cac so la carId co trong JSON; toi da 8 id, khong trung. " +
                "Neu khong chac carId thi khong them dong nay.";

            if (provider.Equals("Gemini", StringComparison.OrdinalIgnoreCase))
            {
                var geminiKey = _config["AiAdvisor:GeminiApiKey"];
                var fallbackKey = _config["AiAdvisor:OpenAIApiKey"];
                var apiKey = !string.IsNullOrWhiteSpace(geminiKey) ? geminiKey : fallbackKey;

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    throw new InvalidOperationException(
                        "Chua cau hinh AiAdvisor:GeminiApiKey. " +
                        "Them khoa Google AI vao User Secrets hoac appsettings.");
                }

                var model = ResolveGeminiModel(_config["AiAdvisor:Model"]);
                var geminiReply = await GetReplyGeminiAsync(request, systemPrompt, apiKey.Trim(), model);
                return ApplySuggestedCarIds(geminiReply);
            }

            var openAiReply = await GetReplyOpenAiAsync(request, systemPrompt);
            return ApplySuggestedCarIds(openAiReply);
        }

        // ─────────────────────────────────────────────
        // Bóc tách suggestedCarIds từ dòng ẩn <!--AI_CAR_IDS:...-->
        // ─────────────────────────────────────────────

        private static AiAdvisorChatResponseDto ApplySuggestedCarIds(AiAdvisorChatResponseDto dto)
        {
            var (text, ids) = ExtractSuggestedCarIds(dto.Reply);
            dto.Reply = text;
            dto.SuggestedCarIds = ids;
            return dto;
        }

        /// <summary>
        /// Bóc dòng <!--AI_CAR_IDS:id1,id2--> do model thêm,
        /// trả về nội dung sạch và danh sách carId để FE render thẻ xe.
        /// </summary>
        private static (string reply, List<int>? ids) ExtractSuggestedCarIds(string reply)
        {
            if (string.IsNullOrWhiteSpace(reply))
                return (reply, null);

            // Match dòng <!--AI_CAR_IDS:12,34-->
            var match = Regex.Match(
                reply,
                @"<!--AI_CAR_IDS:([\d,]+)-->",
                RegexOptions.CultureInvariant);

            if (!match.Success)
                return (reply.TrimEnd(), null);

            // Parse danh sách carId
            var ids = new List<int>();
            foreach (var part in match.Groups[1].Value.Split(',',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (int.TryParse(part, NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out var id) && id > 0)
                {
                    ids.Add(id);
                }
            }

            ids = ids.Distinct().Take(8).ToList();

            // Xóa hoàn toàn dòng ẩn khỏi nội dung hiển thị
            var cleaned = Regex.Replace(
                reply,
                @"\s*<!--AI_CAR_IDS:[\d,]+-->\s*$",
                "",
                RegexOptions.CultureInvariant).TrimEnd();

            return (cleaned, ids.Count > 0 ? ids : null);
        }

        // ─────────────────────────────────────────────
        // Resolve model name
        // ─────────────────────────────────────────────

        private static string ResolveGeminiModel(string? configured)
        {
            var m = (configured ?? "").Trim();
            if (string.IsNullOrEmpty(m) || m.StartsWith("gpt-", StringComparison.OrdinalIgnoreCase))
                return "gemini-2.5-flash";

            if (m.Contains("1.5", StringComparison.OrdinalIgnoreCase))
                return "gemini-2.5-flash";

            return m;
        }

        // ─────────────────────────────────────────────
        // OpenAI
        // ─────────────────────────────────────────────

        private async Task<AiAdvisorChatResponseDto> GetReplyOpenAiAsync(
            AiAdvisorChatRequestDto request,
            string systemPrompt)
        {
            var apiKey = _config["AiAdvisor:OpenAIApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    "Chua cau hinh AiAdvisor:OpenAIApiKey. " +
                    "Neu dung Gemini, dat AiAdvisor:Provider thanh Gemini va dien AiAdvisor:GeminiApiKey.");
            }

            var model = _config["AiAdvisor:Model"] ?? "gpt-4o-mini";
            var baseUrl = (_config["AiAdvisor:BaseUrl"] ?? "https://api.openai.com/v1").TrimEnd('/');
            var messages = BuildOpenAiMessages(request, systemPrompt);

            var payload = new Dictionary<string, object?>
            {
                ["model"] = model,
                ["messages"] = messages,
                ["temperature"] = 0.55
            };

            var req = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/chat/completions")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json")
            };
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey.Trim());

            _http.Timeout = TimeSpan.FromSeconds(90);
            var response = await _http.SendAsync(req);
            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"OpenAI HTTP {(int)response.StatusCode}: {responseText}");

            using var doc = JsonDocument.Parse(responseText);
            var choices = doc.RootElement.GetProperty("choices");
            if (choices.GetArrayLength() == 0)
                return new AiAdvisorChatResponseDto { Reply = "Xin loi, toi khong nhan duoc cau tra loi tu dich vu AI. Vui long thu lai." };

            var reply = choices[0].GetProperty("message").GetProperty("content").GetString() ?? "";
            return new AiAdvisorChatResponseDto { Reply = reply };
        }

        // ─────────────────────────────────────────────
        // Gemini
        // ─────────────────────────────────────────────

        private async Task<AiAdvisorChatResponseDto> GetReplyGeminiAsync(
            AiAdvisorChatRequestDto request,
            string systemPrompt,
            string apiKey,
            string model)
        {
            var apiVersion = (_config["AiAdvisor:GeminiApiVersion"] ?? "v1beta").Trim().Trim('/');
            if (apiVersion.Length == 0)
                apiVersion = "v1beta";

            var contents = BuildGeminiContents(request);
            Dictionary<string, object?> payload;

            if (apiVersion.Equals("v1", StringComparison.OrdinalIgnoreCase))
            {
                var withSystem = new List<Dictionary<string, object?>>
                {
                    new()
                    {
                        ["role"] = "user",
                        ["parts"] = new object[] { new Dictionary<string, string> { ["text"] = systemPrompt } }
                    },
                    new()
                    {
                        ["role"] = "model",
                        ["parts"] = new object[]
                        {
                            new Dictionary<string, string>
                            {
                                ["text"] = "Da hieu. Toi se tuan theo huong dan va chi dung du lieu JSON he thong ban cung cap khi tu van."
                            }
                        }
                    }
                };
                withSystem.AddRange(contents);
                payload = new Dictionary<string, object?>
                {
                    ["contents"] = withSystem,
                    ["generationConfig"] = new Dictionary<string, object?> { ["temperature"] = 0.55d }
                };
            }
            else
            {
                payload = new Dictionary<string, object?>
                {
                    ["systemInstruction"] = new Dictionary<string, object?>
                    {
                        ["parts"] = new object[] { new Dictionary<string, string> { ["text"] = systemPrompt } }
                    },
                    ["contents"] = contents,
                    ["generationConfig"] = new Dictionary<string, object?> { ["temperature"] = 0.55d }
                };
            }

            var json = JsonSerializer.Serialize(payload);
            var url = $"https://generativelanguage.googleapis.com/{apiVersion}/models/" +
                      $"{Uri.EscapeDataString(model)}:generateContent?key={Uri.EscapeDataString(apiKey)}";

            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            _http.Timeout = TimeSpan.FromSeconds(90);
            var response = await _http.SendAsync(req);
            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Gemini HTTP {(int)response.StatusCode}: {responseText}");

            using var doc = JsonDocument.Parse(responseText);

            // Kiểm tra nội dung bị block
            if (doc.RootElement.TryGetProperty("promptFeedback", out var pf) &&
                pf.TryGetProperty("blockReason", out var br) &&
                br.ValueKind == JsonValueKind.String &&
                br.GetString() is { } reason &&
                !string.IsNullOrEmpty(reason))
            {
                return new AiAdvisorChatResponseDto
                {
                    Reply = $"Noi dung khong duoc mo hinh xu ly. Ban thu dien dat ngan gon hon hoac lien he hotline."
                };
            }

            if (!doc.RootElement.TryGetProperty("candidates", out var candidates) ||
                candidates.GetArrayLength() == 0)
            {
                return new AiAdvisorChatResponseDto
                {
                    Reply = "Xin loi, toi khong nhan duoc cau tra loi tu Gemini. Vui long thu lai."
                };
            }

            var content = candidates[0].GetProperty("content");
            if (!content.TryGetProperty("parts", out var partsEl) || partsEl.GetArrayLength() == 0)
            {
                return new AiAdvisorChatResponseDto
                {
                    Reply = "Xin loi, phan hoi tu Gemini khong co noi dung van ban."
                };
            }

            var sb = new StringBuilder();
            foreach (var part in partsEl.EnumerateArray())
            {
                if (part.TryGetProperty("text", out var t) && t.GetString() is { } fragment)
                    sb.Append(fragment);
            }

            var reply = sb.ToString();
            return string.IsNullOrWhiteSpace(reply)
                ? new AiAdvisorChatResponseDto { Reply = "Xin loi, toi khong nhan duoc cau tra loi tu Gemini. Vui long thu lai." }
                : new AiAdvisorChatResponseDto { Reply = reply };
        }

        // ─────────────────────────────────────────────
        // Build message lists
        // ─────────────────────────────────────────────

        private static List<Dictionary<string, object?>> BuildGeminiContents(AiAdvisorChatRequestDto request)
        {
            var list = new List<Dictionary<string, object?>>();
            var skipLeadingAssistant = true;

            if (request.History is { Count: > 0 })
            {
                // Tối ưu token: chỉ gửi 6 tin nhắn gần nhất
                foreach (var turn in request.History.TakeLast(6))
                {
                    if (skipLeadingAssistant &&
                        string.Equals(turn.Role, "assistant", StringComparison.OrdinalIgnoreCase))
                        continue;

                    skipLeadingAssistant = false;
                    var role = string.Equals(turn.Role, "assistant", StringComparison.OrdinalIgnoreCase)
                        ? "model" : "user";

                    list.Add(new Dictionary<string, object?>
                    {
                        ["role"] = role,
                        ["parts"] = new object[] { new Dictionary<string, string> { ["text"] = turn.Content ?? "" } }
                    });
                }
            }

            list.Add(new Dictionary<string, object?>
            {
                ["role"] = "user",
                ["parts"] = new object[] { new Dictionary<string, string> { ["text"] = request.Message.Trim() } }
            });

            return list;
        }

        private static List<Dictionary<string, string>> BuildOpenAiMessages(
            AiAdvisorChatRequestDto request,
            string systemPrompt)
        {
            var messages = new List<Dictionary<string, string>>
            {
                new() { ["role"] = "system", ["content"] = systemPrompt }
            };

            if (request.History is { Count: > 0 })
            {
                // Tối ưu token: chỉ gửi 6 tin nhắn gần nhất
                foreach (var turn in request.History.TakeLast(6))
                {
                    var role = turn.Role?.ToLowerInvariant() == "assistant" ? "assistant" : "user";
                    messages.Add(new Dictionary<string, string>
                    {
                        ["role"] = role,
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

        // ─────────────────────────────────────────────
        // Build KnowledgeBase JSON từ DB
        // ─────────────────────────────────────────────

        private async Task<string> BuildKnowledgeBaseAsync(int maxCars)
        {
            object? cars = null;
            object? showrooms = null;
            object? articles = null;
            object? carVersions = null;

            try
            {
                var rawCars = await _carService.GetCarsAsync(
                    search: null, brand: null, color: null,
                    minPrice: null, maxPrice: null, status: null,
                    transmission: null, bodyStyle: null, fuelType: null,
                    location: null, condition: null, minYear: null, maxYear: null,
                    sort: null, inStockOnly: false, page: 1, pageSize: maxCars);

                cars = rawCars;
            }
            catch { }

            try
            {
                var rawShowrooms = await _showroomService.GetAllShowroomsAsync();

                // Tối ưu token: chỉ lấy các trường cần thiết
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
                articles = await _articleService.GetArticlesAdminAsync(null, 1, 5, true);
            }
            catch { }

            try
            {
                var rawVersions = await _pricingAdminService.GetAllAsync(null, true);

                // Tối ưu token: chỉ lấy ID, tên phiên bản và giá
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

            return JsonSerializer.Serialize(finalKnowledge, new JsonSerializerOptions
            {
                WriteIndented = false,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            });
        }
    }
}