using System.Net;
using System.Text.Json;
using SentinelApp.Application.Core;

namespace SentinelApp.API.Middleware
{
    public class ValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ValidationMiddleware> _logger;

        public ValidationMiddleware(RequestDelegate next, ILogger<ValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip validation for certain endpoints
            if (ShouldSkipValidation(context))
            {
                await _next(context);
                return;
            }

            // Check if ModelState exists and is invalid
            if (context.Items.TryGetValue("ModelState", out var modelStateObj) &&
                modelStateObj is Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState &&
                !modelState.IsValid)
            {
                await HandleValidationErrors(context, modelState);
                return;
            }

            await _next(context);
        }

        private bool ShouldSkipValidation(HttpContext context)
        {
            var path = context.Request.Path.Value ?? "";

            // Skip validation for specific endpoints
            var skipPaths = new[] { "/health", "/metrics", "/swagger", "/favicon.ico" };
            return skipPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
        }

        private async Task HandleValidationErrors(HttpContext context, Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState)
        {
            var validationErrors = new Dictionary<string, string[]>();

            foreach (var key in modelState.Keys)
            {
                var errors = modelState[key]?.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                if (errors != null && errors.Length > 0)
                {
                    // Convert property names to camelCase for consistency
                    var camelCaseKey = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(key);
                    validationErrors[camelCaseKey] = errors;
                }
            }

            var errorResponse = new ServiceResponse<object>
            {
                Success = false,
                HttpStatusCode = HttpStatusCode.BadRequest,
                HttpStatusCodeNumber = (int)HttpStatusCode.BadRequest,
                ResponseMessage = "One or more validation errors occurred.",
                ValidationErrors = validationErrors
            };

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            await context.Response.WriteAsync(json);
        }
    }
}
