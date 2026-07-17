using Microsoft.AspNetCore.Mvc.Filters;

namespace SentinelApp.API.Filters
{
    public class ModelStateCaptureFilter : ActionFilterAttribute
    {
        private readonly ILogger<ModelStateCaptureFilter> _logger;

        public ModelStateCaptureFilter(ILogger<ModelStateCaptureFilter> logger)
        {
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("ModelStateCaptureFilter: Capturing ModelState");

            // Store ModelState in HttpContext so middleware can access it
            context.HttpContext.Items["ModelState"] = context.ModelState;

            // Log validation state
            _logger.LogInformation($"ModelState IsValid: {context.ModelState.IsValid}");
            if (!context.ModelState.IsValid)
            {
                foreach (var key in context.ModelState.Keys)
                {
                    var errors = context.ModelState[key]?.Errors;
                    if (errors != null && errors.Count > 0)
                    {
                        _logger.LogInformation($"Validation errors for {key}: {string.Join(", ", errors.Select(e => e.ErrorMessage))}");
                    }
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
