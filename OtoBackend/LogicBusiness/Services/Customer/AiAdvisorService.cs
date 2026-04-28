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

            var phoneMatch = Regex.Match(request.Message, @"\b0[35789]\d{8}\b");

            if (phoneMatch.Success)
            {
                var phone = phoneMatch.Value;

                // Kích hoạt Notification bắn ngay cho team Sales
                await _notificationService.CreateNotificationAsync(
                    userId: null,
                    showroomId: null, // Nếu ní có logic bắt ID showroom thì truyền vào, không thì để null bắn cho Sales tổng
                    roleTarget: AppRoles.Sales, // Nhớ tạo file AppRoles như tin nhắn trước nhé
                    title: "🚨 Khách hàng để lại SĐT qua AI Chat",
                    content: $"SĐT: {phone}. Khách nhắn: \"{request.Message}\". Mau gọi chốt đơn!",
                    actionUrl: "/admin/leads", // Ní sửa thành link trang quản lý khách hàng của UI Admin
                    type: "LEAD"
                );
            }

            var provider = (_config["AiAdvisor:Provider"] ?? "OpenAI").Trim();
            if (!provider.Equals("Gemini", StringComparison.OrdinalIgnoreCase)
                && !provider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase))
            {
                if (provider.StartsWith("AIza", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "AiAdvisor:Provider phải là chữ Gemini hoặc OpenAI, không phải API key. Đặt khóa Google vào AiAdvisor:GeminiApiKey và \"Provider\": \"Gemini\".");
                }

                throw new InvalidOperationException(
                    $"AiAdvisor:Provider không hợp lệ ({provider}). Chỉ dùng \"Gemini\" hoặc \"OpenAI\".");
            }

            var maxCars = Math.Clamp(int.TryParse(_config["AiAdvisor:MaxCatalogCars"], out var n) ? n : 60, 1, 100);

            // Lấy toàn bộ dữ liệu tổng hợp thay vì chỉ xe
            var knowledgeBaseJson = await BuildKnowledgeBaseAsync(maxCars);

            // Cập nhật lại System Prompt để AI hiểu cấu trúc JSON mới
            var systemPrompt =
                "Bạn là trợ lý tư vấn tại showroom ô tô, trả lời bằng tiếng Việt.\n" +
                "Chỉ dựa trên thông tin trong khối JSON dưới đây khi tư vấn về: danh sách xe, giá cả, phiên bản, thông tin tồn kho, hệ thống showroom, và các bài viết/sự kiện mới nhất.\n" +
                "LƯU Ý CỰC KỲ QUAN TRỌNG: Để biết xe có những phiên bản nào, hãy tìm 'carId' của xe đó trong mảng 'Cars', sau đó tìm tất cả các phiên bản có cùng 'carId' nằm trong mảng 'CarVersions'.\n" +
                "Nếu không có thông tin trong dữ liệu, hãy nói rõ bạn không nắm thông tin đó và mời khách gọi hotline hoặc để lại liên hệ.\n" +
                "Không được tự bịa số liệu, địa chỉ hay sự kiện. Giữ giọng điệu thân thiện, rõ ràng, chuyên nghiệp.\n\n" +
                "DỮ LIỆU HỆ THỐNG (JSON chứa Cars, CarVersions, Showrooms, Articles):\n" +
                knowledgeBaseJson +
                "\n\nQUY ƯỚC MÁY ĐỌC (bắt buộc khi liệt kê xe cụ thể): Mỗi xe trong JSON có trường carId. " +
                "Khi bạn đề xuất hoặc liệt kê các xe cụ thể từ JSON, thêm ĐÚNG một dòng tuyệt đối CUỐI cùng của tin nhắn, không có ký tự nào sau đó, định dạng: " +
                "với các số là carId có trong JSON; tối đa 8 id, không trùng. Nếu không chắc carId thì không thêm dòng này.";

            if (provider.Equals("Gemini", StringComparison.OrdinalIgnoreCase))
            {
                var geminiKey = _config["AiAdvisor:GeminiApiKey"];
                var fallbackKey = _config["AiAdvisor:OpenAIApiKey"];
                var apiKey = !string.IsNullOrWhiteSpace(geminiKey) ? geminiKey : fallbackKey;
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    throw new InvalidOperationException(
                        "Chưa cấu hình AiAdvisor:GeminiApiKey (hoặc tạm dùng AiAdvisor:OpenAIApiKey). Thêm khóa Google AI vào User Secrets hoặc appsettings.");
                }

                var model = ResolveGeminiModel(_config["AiAdvisor:Model"]);
                var geminiReply = await GetReplyGeminiAsync(request, systemPrompt, apiKey.Trim(), model);
                return ApplySuggestedCarIds(geminiReply);
            }

            var openAiReply = await GetReplyOpenAiAsync(request, systemPrompt);
            return ApplySuggestedCarIds(openAiReply);
        }

        private static AiAdvisorChatResponseDto ApplySuggestedCarIds(AiAdvisorChatResponseDto dto)
        {
            var (text, ids) = ExtractSuggestedCarIds(dto.Reply);
            dto.Reply = text;
            dto.SuggestedCarIds = ids;
            return dto;
        }

        /// <summary>Bóc dòng do model thêm để client gọi API hiển thị thẻ xe.</summary>
        private static (string reply, List<int>? ids) ExtractSuggestedCarIds(string reply)
        {
            if (string.IsNullOrWhiteSpace(reply))
            {
                return (reply, null);
            }

            // Đã fix lại Regex để xóa triệt để ID ẩn
            var match = Regex.Match(reply, @"", RegexOptions.CultureInvariant);
            if (!match.Success)
            {
                return (reply.TrimEnd(), null);
            }

            var ids = new List<int>();
            foreach (var part in match.Groups[1].Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (int.TryParse(part, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id) && id > 0)
                {
                    ids.Add(id);
                }
            }

            ids = ids.Distinct().Take(8).ToList();

            // Đã fix Regex để xóa dòng
            var cleaned = Regex.Replace(reply, @"\s*\s*$", "", RegexOptions.CultureInvariant).TrimEnd();
            return (cleaned, ids.Count > 0 ? ids : null);
        }

        private static string ResolveGeminiModel(string? configured)
        {
            var m = (configured ?? "").Trim();
            if (string.IsNullOrEmpty(m) || m.StartsWith("gpt-", StringComparison.OrdinalIgnoreCase))
            {
                return "gemini-2.5-flash";
            }

            if (m.Contains("1.5", StringComparison.OrdinalIgnoreCase))
            {
                return "gemini-2.5-flash";
            }

            return m;
        }

        private async Task<AiAdvisorChatResponseDto> GetReplyOpenAiAsync(
            AiAdvisorChatRequestDto request,
            string systemPrompt)
        {
            var apiKey = _config["AiAdvisor:OpenAIApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    "Chưa cấu hình AiAdvisor:OpenAIApiKey. Nếu dùng Gemini, đặt AiAdvisor:Provider thành Gemini và điền AiAdvisor:GeminiApiKey.");
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
            {
                throw new HttpRequestException($"OpenAI HTTP {(int)response.StatusCode}: {responseText}");
            }

            using var doc = JsonDocument.Parse(responseText);
            var choices = doc.RootElement.GetProperty("choices");
            if (choices.GetArrayLength() == 0)
            {
                return new AiAdvisorChatResponseDto { Reply = "Xin lỗi, tôi không nhận được câu trả lời từ dịch vụ AI. Vui lòng thử lại." };
            }

            var reply = choices[0].GetProperty("message").GetProperty("content").GetString() ?? "";
            return new AiAdvisorChatResponseDto { Reply = reply };
        }

        private async Task<AiAdvisorChatResponseDto> GetReplyGeminiAsync(
            AiAdvisorChatRequestDto request,
            string systemPrompt,
            string apiKey,
            string model)
        {
            var apiVersion = (_config["AiAdvisor:GeminiApiVersion"] ?? "v1beta").Trim().Trim('/');
            if (apiVersion.Length == 0)
            {
                apiVersion = "v1beta";
            }

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
                                ["text"] = "Đã hiểu. Tôi sẽ tuân theo hướng dẫn và chỉ dùng dữ liệu JSON hệ thống bạn cung cấp khi tư vấn."
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

            var url =
                $"https://generativelanguage.googleapis.com/{apiVersion}/models/{Uri.EscapeDataString(model)}:generateContent?key={Uri.EscapeDataString(apiKey)}";

            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            _http.Timeout = TimeSpan.FromSeconds(90);
            var response = await _http.SendAsync(req);
            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Gemini HTTP {(int)response.StatusCode}: {responseText}");
            }

            using var doc = JsonDocument.Parse(responseText);
            if (doc.RootElement.TryGetProperty("promptFeedback", out var pf) &&
                pf.TryGetProperty("blockReason", out var br) &&
                br.ValueKind == JsonValueKind.String &&
                br.GetString() is { } reason &&
                !string.IsNullOrEmpty(reason))
            {
                return new AiAdvisorChatResponseDto
                {
                    Reply = $"Nội dung không được mô hình xử lý ({reason}). Bạn thử diễn đạt ngắn gọn hơn hoặc liên hệ hotline."
                };
            }

            if (!doc.RootElement.TryGetProperty("candidates", out var candidates) ||
                candidates.GetArrayLength() == 0)
            {
                return new AiAdvisorChatResponseDto { Reply = "Xin lỗi, tôi không nhận được câu trả lời từ Gemini. Vui lòng thử lại." };
            }

            var content = candidates[0].GetProperty("content");
            if (!content.TryGetProperty("parts", out var partsEl) || partsEl.GetArrayLength() == 0)
            {
                return new AiAdvisorChatResponseDto { Reply = "Xin lỗi, phản hồi từ Gemini không có nội dung văn bản." };
            }

            var sb = new StringBuilder();
            foreach (var part in partsEl.EnumerateArray())
            {
                if (part.TryGetProperty("text", out var t) && t.GetString() is { } fragment)
                {
                    sb.Append(fragment);
                }
            }

            var reply = sb.ToString();
            return string.IsNullOrWhiteSpace(reply)
                ? new AiAdvisorChatResponseDto { Reply = "Xin lỗi, tôi không nhận được câu trả lời từ Gemini. Vui lòng thử lại." }
                : new AiAdvisorChatResponseDto { Reply = reply };
        }

        private static List<Dictionary<string, object?>> BuildGeminiContents(AiAdvisorChatRequestDto request)
        {
            var list = new List<Dictionary<string, object?>>();
            var skipLeadingAssistant = true;

            if (request.History is { Count: > 0 })
            {
                // TỐI ƯU TOKEN: Chỉ gửi 6 tin nhắn gần nhất thay vì 20
                foreach (var turn in request.History.TakeLast(6))
                {
                    if (skipLeadingAssistant && string.Equals(turn.Role, "assistant", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    skipLeadingAssistant = false;
                    var role = string.Equals(turn.Role, "assistant", StringComparison.OrdinalIgnoreCase) ? "model" : "user";
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
                // TỐI ƯU TOKEN: Chỉ gửi 6 tin nhắn gần nhất thay vì 20
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

        private async Task<string> BuildKnowledgeBaseAsync(int maxCars)
        {
            object? cars = null;
            object? showrooms = null;
            object? articles = null;
            object? carVersions = null;

            try
            {
                var rawCars = await _carService.GetCarsAsync(
                    search: null, brand: null, color: null, minPrice: null, maxPrice: null,
                    status: null, transmission: null, bodyStyle: null, fuelType: null,
                    location: null, condition: null, minYear: null, maxYear: null,
                    sort: null, inStockOnly: false, page: 1, pageSize: maxCars);

                // Tạm thời truyền nguyên list Cars để tránh lỗi cấu trúc của PagedResult.
                cars = rawCars;
            }
            catch { }

            try
            {
                var rawShowrooms = await _showroomService.GetAllShowroomsAsync();

                // TỐI ƯU TOKEN: Chỉ lấy Tên, Hotline, Địa chỉ thay vì lấy toàn bộ Object
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
                // TỐI ƯU TOKEN: Lấy dữ liệu bài viết (nếu cần gọt dũa, bạn có thể .Select thêm ở đây)
                articles = await _articleService.GetArticlesAdminAsync(null, 1, 5, true);
            }
            catch { }

            try
            {
                var rawVersions = await _pricingAdminService.GetAllAsync(null, true);

                // TỐI ƯU TOKEN: Chỉ lấy ID, Tên phiên bản và Giá
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