using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Domain.Exceptions;
using RealEstateSystem.Infrastructure.Configuration;
using RealEstateSystem.Infrastructure.Services;
using Xunit;

namespace RealEstateSystem.Application.UnitTests.Services
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _sendAsyncFunc;

        public MockHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> sendAsyncFunc)
        {
            _sendAsyncFunc = sendAsyncFunc;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _sendAsyncFunc(request);
        }
    }

    public class GeminiAiServiceTests
    {
        [Fact]
        public async Task OptimizeListingAsync_WhenApiKeyIsEmpty_ShouldThrowBadRequestException()
        {
            var settings = new GeminiSettings { ApiKey = "" };
            var options = Options.Create(settings);
            var service = new GeminiAiService(options);

            var request = new OptimizeListingRequest { Title = "Title", Description = "Desc" };

            var act = () => service.OptimizeListingAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("Cấu hình API Key cho Gemini chưa được thiết lập.");
        }

        [Fact]
        public async Task OptimizeListingAsync_WhenApiCallIsSuccessful_ShouldReturnOptimizedListing()
        {
            var settings = new GeminiSettings { ApiKey = "ValidKey", Model = "gemini-1.5-flash" };
            var options = Options.Create(settings);

            var mockResponseJson = @"
            {
                ""candidates"": [
                    {
                        ""content"": {
                            ""parts"": [
                                {
                                    ""text"": ""{ \\""Title\\"": \\""Tieu de SEO\\"", \\""Description\\"": \\""Mo ta SEO\\"" }""
                                }
                            ]
                        }
                    }
                ]
            }";

            var handler = new MockHttpMessageHandler(req =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(mockResponseJson, Encoding.UTF8, "application/json")
                };
                return Task.FromResult(response);
            });

            var httpClient = new HttpClient(handler);
            var service = new GeminiAiService(options);

            typeof(GeminiAiService).GetField("_httpClient", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(null, httpClient);

            var request = new OptimizeListingRequest
            {
                Title = "Original Title",
                Description = "Original Desc",
                Price = 1000000,
                Area = 50
            };

            var result = await service.OptimizeListingAsync(request, CancellationToken.None);

            result.Should().NotBeNull();
            result.Title.Should().Be("Tieu de SEO");
            result.Description.Should().Be("Mo ta SEO");
        }

        [Fact]
        public async Task OptimizeListingAsync_WhenApiCallFails_ShouldThrowBadRequestException()
        {
            var settings = new GeminiSettings { ApiKey = "ValidKey", Model = "gemini-1.5-flash" };
            var options = Options.Create(settings);

            var handler = new MockHttpMessageHandler(req =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Model not found", Encoding.UTF8, "text/plain")
                };
                return Task.FromResult(response);
            });

            var httpClient = new HttpClient(handler);
            var service = new GeminiAiService(options);

            typeof(GeminiAiService).GetField("_httpClient", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(null, httpClient);

            var request = new OptimizeListingRequest { Title = "Title", Description = "Desc" };

            var act = () => service.OptimizeListingAsync(request, CancellationToken.None);

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("Lỗi kết nối với Gemini AI API: Model not found");
        }
    }
}
