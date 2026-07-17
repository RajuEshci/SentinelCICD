using Microsoft.AspNetCore.Http;
using SentinelApp.Application.Interfaces;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace SentinelApp.API.Middleware
{
    public class RequestDecryptMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEncryptionService _encryptionService;
        private readonly IConfiguration _configuration;

        public RequestDecryptMiddleware(RequestDelegate next, IEncryptionService encryptionService, IConfiguration configuration    )
        {
            _next = next;
            _encryptionService = encryptionService;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                bool encryptionEnabled = _configuration.GetValue<bool>("EncryptionSettings:Enabled");

                if (!encryptionEnabled)
                {
                    await _next(context);
                    return;
                }
                if (context.Request.Method == HttpMethods.Get)
                {
                    if (context.Request.Query.TryGetValue("encdata", out var encryptedValue))
                    {
                        var decrypted = _encryptionService.Decrypt(encryptedValue);

                        if (!string.IsNullOrWhiteSpace(decrypted))
                        {
							context.Items["EncryptedRequest"] = encryptedValue;
							context.Items["ReadableRequest"] = decrypted;
							context.Request.QueryString = new QueryString("?" + decrypted);
                        }
                    }
                }
                else
                {
                    context.Request.EnableBuffering();

                    using var reader = new StreamReader(
                        context.Request.Body,
                        Encoding.UTF8,
                        leaveOpen: true);

                    var encryptedBody = await reader.ReadToEndAsync();

                    context.Request.Body.Position = 0;

                    if (!string.IsNullOrWhiteSpace(encryptedBody))
                    {
                        var decryptedBody = _encryptionService.Decrypt(encryptedBody);

                        if (!string.IsNullOrWhiteSpace(decryptedBody))
                        {
							context.Items["EncryptedRequest"] = encryptedBody;
							context.Items["ReadableRequest"] = decryptedBody;
							context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(decryptedBody));
                        }
                    }
                }

                await _next(context);
            }
            catch (FormatException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    Success = false,
                    ResponseMessage = "Invalid encrypted request. The provided data is not a valid Base64 string.",
                    HttpStatusCode = HttpStatusCode.BadRequest
                });
            }
            catch (CryptographicException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    Success = false,
                    ResponseMessage = "Invalid encrypted request. Unable to decrypt the data.",
                    HttpStatusCode = HttpStatusCode.BadRequest
                });
            }
        }
    }
}