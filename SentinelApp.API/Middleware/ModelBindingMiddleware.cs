namespace SentinelApp.API.Middleware
{
    public class ModelBindingMiddleware
    {
        private readonly RequestDelegate _next;

        public ModelBindingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();
            await _next(context);
        }
    }
}
