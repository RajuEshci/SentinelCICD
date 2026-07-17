using Microsoft.AspNetCore.Http;
using SentinelApp.Application.Interfaces;
using System.Text;

namespace SentinelApp.API.Middleware
{
    public class ResponseEncryptMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEncryptionService _encryptionService;
        private readonly IConfiguration _configuration;

        public ResponseEncryptMiddleware(RequestDelegate next, IEncryptionService encryptionService, IConfiguration configuration)
        {
            _next = next;
            _encryptionService = encryptionService;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            bool encryptionEnabled = _configuration.GetValue<bool>("EncryptionSettings:Enabled");

            if (!encryptionEnabled)
            {
                await _next(context);
                return;
            }
            var originalBody = context.Response.Body;

            await using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            await _next(context);

            memoryStream.Position = 0;

            var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
			context.Items["ReadableResponse"] = responseBody;
			context.Response.Body = originalBody;

            // Don't encrypt if there is no response
            if (string.IsNullOrWhiteSpace(responseBody))
            {
                return;
            }

            var contentType = context.Response.ContentType ?? string.Empty;

            // ================================
            // Return HTML without encryption
            // ================================
            if (contentType.StartsWith("text/html", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.ContentLength = Encoding.UTF8.GetByteCount(responseBody);
                await context.Response.WriteAsync(responseBody);
                return;
            }

            // ==========================================
            // Encrypt only JSON responses
            // ==========================================
            if (contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
            {
                var encrypted = _encryptionService.Encrypt(responseBody);

                context.Response.ContentType = "text/plain";
                context.Response.ContentLength = Encoding.UTF8.GetByteCount(encrypted);

                await context.Response.WriteAsync(encrypted);
                return;
            }

            // ==========================================
            // For everything else (pdf, image, file...)
            // ==========================================
            memoryStream.Position = 0;
            await memoryStream.CopyToAsync(originalBody);
        }
    }
}