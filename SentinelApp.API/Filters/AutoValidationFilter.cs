using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SentinelApp.Application.Core;

namespace SentinelApp.API.Filters
{
    public class AutoValidationFilter : IAsyncActionFilter
    {
        private readonly ILogger<AutoValidationFilter> _logger;

        public AutoValidationFilter(ILogger<AutoValidationFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //_logger.LogInformation("AutoValidationFilter: Checking ModelState");

            if (!context.ModelState.IsValid)
            {
                //_logger.LogWarning("Model validation failed");

                var validationErrors = new Dictionary<string, string[]>();

                foreach (var key in context.ModelState.Keys)
                {
                    var errors = context.ModelState[key]?.Errors
                        .Select(e => e.ErrorMessage)
                        .ToArray();

                    if (errors != null && errors.Length > 0)
                    {
                        validationErrors[key] = errors;
                    }
                }

                var errorResponse = ServiceResponse<object>.ValidationErrorResponse(
                    "One or more validation errors occurred.",
                    validationErrors
                );

                context.Result = new BadRequestObjectResult(errorResponse);
                return;
            }

            //_logger.LogInformation("Model validation passed");
            await next();
        }
    }
}
