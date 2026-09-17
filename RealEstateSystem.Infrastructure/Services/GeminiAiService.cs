using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Exceptions;
using RealEstateSystem.Infrastructure.Configuration;

namespace RealEstateSystem.Infrastructure.Services
{
    public class GeminiAiService : IAiService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly GeminiSettings _settings;

        public GeminiAiService(IOptions<GeminiSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<OptimizeListingResponse> OptimizeListingAsync(OptimizeListingRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_settings.ApiKey))
            {
                throw new BadRequestException("Cấu hình API Key cho Gemini chưa được thiết lập.");
            }

            var model = string.IsNullOrEmpty(_settings.Model) ? "gemini-1.5-flash" : _settings.Model;
            if (model.StartsWith("models/"))
            {
                model = model.Substring(7);
            }
            var url = $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent?key={_settings.ApiKey}";

            var promptText = "Bạn là một chuyên gia viết nội dung quảng cáo Bất động sản chuẩn SEO tại Việt Nam.\n" +
                             "Hãy tối ưu lại tiêu đề và mô tả của bất động sản sau đây để hấp dẫn người mua và thân thiện với SEO hơn.\n" +
                             "Yêu cầu:\n" +
                             "1. Tiêu đề (Title): Tối đa 70 ký tự, chứa từ khóa chính liên quan đến loại hình và khu vực, thu hút người click.\n" +
                             "2. Mô tả (Description): Viết chi tiết, chuyên nghiệp, chia bố cục rõ ràng (Tổng quan, Điểm nổi bật, Thông số chi tiết diện tích/giá, Lời kêu gọi hành động liên hệ xem nhà).\n" +
                             "3. Trả về ĐÚNG định dạng JSON có cấu trúc chính xác sau: { \"Title\": \"tiêu đề mới\", \"Description\": \"mô tả mới\" }.\n" +
                             "Tuyệt đối không giải thích gì thêm ngoài JSON này. Hãy chỉ trả về khối JSON, không viết lời dẫn hay bất kỳ nội dung nào khác.\n\n" +
                             $"Dữ liệu gốc:\n" +
                             $"- Tiêu đề gốc: {request.Title}\n" +
                             $"- Mô tả gốc: {request.Description}\n" +
                             $"- Giá: {request.Price:N0} VND\n" +
                             $"- Diện tích: {request.Area} m2";

            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = promptText }
                        }
                    }
                }
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var httpResponse = await _httpClient.PostAsJsonAsync(url, payload, jsonOptions, cancellationToken);
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorMsg = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                throw new BadRequestException($"Lỗi kết nối với Gemini AI API: {errorMsg}");
            }

            var geminiResponse = await httpResponse.Content.ReadFromJsonAsync<GeminiResponseDto>(cancellationToken: cancellationToken);
            var jsonText = geminiResponse?.Candidates?[0]?.Content?.Parts?[0]?.Text;

            if (string.IsNullOrEmpty(jsonText))
            {
                throw new BadRequestException("Không nhận được phản hồi hợp lệ từ Gemini AI.");
            }

            jsonText = jsonText.Trim();
            if (jsonText.StartsWith("```json"))
            {
                jsonText = jsonText.Substring(7);
            }
            else if (jsonText.StartsWith("```"))
            {
                jsonText = jsonText.Substring(3);
            }

            if (jsonText.EndsWith("```"))
            {
                jsonText = jsonText.Substring(0, jsonText.Length - 3);
            }
            jsonText = jsonText.Trim();

            try
            {
                var result = JsonSerializer.Deserialize<OptimizeListingResponse>(jsonText, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result ?? new OptimizeListingResponse
                {
                    Title = request.Title,
                    Description = request.Description
                };
            }
            catch
            {
                return new OptimizeListingResponse
                {
                    Title = request.Title,
                    Description = request.Description
                };
            }
        }

        private class GeminiResponseDto
        {
            public CandidateDto[]? Candidates { get; set; }
        }

        private class CandidateDto
        {
            public ContentDto? Content { get; set; }
        }

        private class ContentDto
        {
            public PartDto[]? Parts { get; set; }
        }

        private class PartDto
        {
            public string? Text { get; set; }
        }
    }
}
