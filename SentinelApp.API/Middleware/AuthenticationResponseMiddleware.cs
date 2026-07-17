using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net;
using System.Text.Json;
using SentinelApp.Application.Core;


namespace SentinelApp.API.Middleware
{
    public class AuthenticationResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthenticationResponseMiddleware> _logger;

        public AuthenticationResponseMiddleware(RequestDelegate next, ILogger<AuthenticationResponseMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalStatusCode = context.Response.StatusCode;
            await _next(context);
            await CheckAuthenticationStatus(context, originalStatusCode);
        }

        private async Task CheckAuthenticationStatus(HttpContext context, int originalStatusCode)
        {
            if (context.Response.HasStarted)
                return;
            if (!context.User.Identity.IsAuthenticated)
            {
                var endpoint = context.GetEndpoint();
                if (endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Authorization.IAllowAnonymous>() == null)
                {
                    _logger.LogWarning("User not authenticated for endpoint: {Endpoint}", endpoint?.DisplayName);

                    await WriteAuthenticationErrorResponse(context, 401,
                        GetUnauthorizedMessage(context));
                    return;
                }
            }
            if (context.Response.StatusCode == 401 || context.Response.StatusCode == 403)
            {
                await WriteAuthenticationErrorResponse(context, context.Response.StatusCode,
                    context.Response.StatusCode == 401 ? GetUnauthorizedMessage(context)
                    : "Access forbidden. You don't have permission to access this resource.");
            }
        }

        private async Task WriteAuthenticationErrorResponse(HttpContext context, int statusCode, string message)
        {
            try
            {
                context.Response.Clear();
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                var errorResponse = new ServiceResponse<object>
                {
                    Success = false,
                    HttpStatusCode = (HttpStatusCode)statusCode,
                    HttpStatusCodeNumber = statusCode,
                    ResponseMessage = message
                };

                var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(json);

                _logger.LogInformation("Returning structured {StatusCode} response: {Message}", statusCode, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write authentication error response");
            }
        }

        private string GetUnauthorizedMessage(HttpContext context)
        {
            var hasAuthorizationHeader = context.Request.Headers.ContainsKey("Authorization");

            if (!hasAuthorizationHeader)
            {
                return "Authentication required. Please provide a valid Bearer token.";
            }

            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader))
            {
                return "Authentication required. Please provide a valid Bearer token.";
            }

            if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return "Invalid token format. Please use 'Bearer {token}' format.";
            }

            return "Unauthorized access. Token is invalid or expired.";
        }
    }
}