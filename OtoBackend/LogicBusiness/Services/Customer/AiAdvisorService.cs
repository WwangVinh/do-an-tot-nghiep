using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using LogicBusiness.DTOs;
using LogicBusiness.Interfaces.Customer;
using Microsoft.Extensions.Configuration;

namespace LogicBusiness.Services.Customer
{
    public class AiAdvisorService : IAiAdvisorService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly ICarService _carService;

        public AiAdvisorService(HttpClient http, IConfiguration config, ICarService carService)
        {
            _http = http;
            _config = config;
            _carService = carService;
        }

        public async Task<AiAdvisorChatResponseDto> GetReplyAsync(AiAdvisorChatRequestDto request)
        {
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
            var catalogJson = await BuildCatalogJsonAsync(maxCars);
            var systemPrompt =
                "Bạn là trợ lý tư vấn xe tại showroom ô tô, trả lời bằng tiếng Việt.\n" +
                "Chỉ dựa trên thông tin trong khối JSON dưới đây khi nói về xe, giá, tồn kho, showroom.\n" +
                "Nếu không có trong dữ liệu, nói rõ bạn không có thông tin và mời khách gọi hotline hoặc để lại liên hệ.\n" +
                "Không bịa số liệu. Giữ giọng thân thiện, rõ ràng.\n\n" +
                "DỮ LIỆU XE (JSON từ hệ thống):\n" +
                catalogJson +
                "\n\nQUY ƯỚC MÁY ĐỌC (bắt buộc khi liệt kê xe cụ thể): Mỗi xe trong JSON có trường carId. " +
                "Khi bạn đề xuất hoặc liệt kê các xe cụ thể từ JSON, thêm ĐÚNG một dòng tuyệt đối CUỐI cùng của tin nhắn, không có ký tự nào sau đó, định dạng: <!--AI_CAR_IDS:12,34,56--> " +
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

        /// <summary>Bóc dòng <!--AI_CAR_IDS:...--> do model thêm để client gọi API hiển thị thẻ xe.</summary>
        private static (string reply, List<int>? ids) ExtractSuggestedCarIds(string reply)
        {
            if (string.IsNullOrWhiteSpace(reply))
            {
                return (reply, null);
            }

            var match = Regex.Match(reply, @"<!--AI_CAR_IDS:([\d,\s]+)-->", RegexOptions.CultureInvariant);
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
            var cleaned = Regex.Replace(reply, @"\s*<!--AI_CAR_IDS:[\d,\s]+-->\s*$", "", RegexOptions.CultureInvariant).TrimEnd();
            return (cleaned, ids.Count > 0 ? ids : null);
        }

        /// <summary>Dùng model còn được Google AI (generativelanguage) phục vụ; 1.5 đã shutdown từ 2025-09-29.</summary>
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

            // REST /v1 không có trường systemInstruction (chỉ v1beta); curl tối giản dùng v1 không gửi system.
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
                                ["text"] =
                                    "Đã hiểu. Tôi sẽ tuân theo hướng dẫn và chỉ dùng dữ liệu JSON xe bạn đã cung cấp khi tư vấn."
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
                foreach (var turn in request.History.TakeLast(20))
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
                foreach (var turn in request.History.TakeLast(20))
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

        private async Task<string> BuildCatalogJsonAsync(int maxCars)
        {
            object catalog;
            try
            {
                catalog = await _carService.GetCarsAsync(
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
            catch
            {
                return "[]";
            }

            return JsonSerializer.Serialize(catalog, new JsonSerializerOptions { WriteIndented = false });
        }
    }
}
