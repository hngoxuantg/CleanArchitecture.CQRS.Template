using System.Text;

namespace Project.API.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            LogRequest(context.Request);
            await _next(context);
            LogResponse(context.Response);
        }
        private void LogRequest(HttpRequest request)
        {
            _logger.LogInformation($"Request received: {request.Method} {request.Path}");
            _logger.LogInformation($"Request headers: {GetHeadersAsString(request.Headers)}");
        }
        private void LogResponse(HttpResponse response)
        {
            _logger.LogInformation($"Response sent: {response.StatusCode}");
            _logger.LogInformation($"Response headers: {GetHeadersAsString(response.Headers)}");
        }
        private string GetHeadersAsString(IHeaderDictionary headers)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var (key, value) in headers)
            {
                stringBuilder.AppendLine($"{key}: {value}");
            }
            return stringBuilder.ToString();
        }
    }
    public static class RequestResponseLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}