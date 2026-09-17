using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RealEstateSystem.Domain.Exceptions;

namespace RealEstateSystem.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var problemDetails = new ProblemDetails
            {
                Instance = context.Request.Path
            };

            if (exception is DomainException domainException)
            {
                context.Response.StatusCode = domainException.StatusCode;
                problemDetails.Status = domainException.StatusCode;
                problemDetails.Title = "Yêu cầu không hợp lệ hoặc lỗi nghiệp vụ.";
                problemDetails.Detail = domainException.Message;
                problemDetails.Type = GetTypeUri(domainException.StatusCode);
            }
            else
            {
                _logger.LogError(exception, "Đã xảy ra lỗi không mong muốn hệ thống tại {Path}", context.Request.Path);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Lỗi hệ thống.";
                problemDetails.Detail = "Đã xảy ra lỗi hệ thống trong quá trình xử lý.";
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var jsonResponse = JsonSerializer.Serialize(problemDetails, options);
            await context.Response.WriteAsync(jsonResponse);
        }

        private string GetTypeUri(int statusCode)
        {
            return statusCode switch
            {
                StatusCodes.Status400BadRequest => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                StatusCodes.Status401Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
                StatusCodes.Status403Forbidden => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                StatusCodes.Status404NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                _ => "https://tools.ietf.org/html/rfc7231"
            };
        }
    }
}
