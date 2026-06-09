using Project.Application.Common.Exceptions;
using Project.Common.Constants;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Project.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        private static readonly JsonSerializerOptions _logJsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private static readonly JsonSerializerOptions _responseJsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

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
            catch (Exception ex)
            {
                await HandleErrorAsync(context, ex);
            }
        }

        private async Task HandleErrorAsync(HttpContext context, Exception ex)
        {
            int statusCode;
            string errorType = ex.GetType().Name;
            string traceId = context.TraceIdentifier;
            string requestPath = context.Request.Path;
            string requestMethod = context.Request.Method;
            object response;

            if (ex is ValidatorException validatorException)
            {
                statusCode = (int)validatorException.HttpStatusCode;

                _logger.LogWarning(ex,
                    "Validation error occurred. " +
                    "TraceId: {TraceId}, " +
                    "StatusCode: {StatusCode}, " +
                    "ErrorCode: {ErrorCode}, " +
                    "ErrorType: {ErrorType}, " +
                    "Message: {Message}, " +
                    "RequestPath: {RequestPath}, " +
                    "RequestMethod: {RequestMethod}, " +
                    "Timestamp: {Timestamp}, " +
                    "ValidationErrors: {ValidationErrors}",
                    traceId,
                    statusCode,
                    validatorException.ErrorCode,
                    errorType,
                    validatorException.Message,
                    requestPath,
                    requestMethod,
                    validatorException.Timestamp,
                    validatorException.ValidationErrors.Any()
                        ? JsonSerializer.Serialize(validatorException.ValidationErrors, _logJsonOptions)
                        : "{}");

                if (validatorException.ValidationErrors.Any())
                {
                    IEnumerable<object> flattenedErrors = validatorException.ValidationErrors
                        .SelectMany(kvp => (kvp.Value ?? [])
                            .Select(msg => new
                            {
                                field = string.IsNullOrEmpty(kvp.Key)
                                    ? kvp.Key
                                    : char.ToLowerInvariant(kvp.Key[0]) + kvp.Key[1..],
                                message = msg
                            }))
                        .ToList();

                    response = new
                    {
                        success = false,
                        message = "One or more validation errors occurred.",
                        errors = flattenedErrors,
                        error = new
                        {
                            code = validatorException.ErrorCode,
                            type = errorType,
                        },
                        traceId,
                        timestamp = validatorException.Timestamp
                    };
                }
                else
                {
                    response = new
                    {
                        success = false,
                        message = validatorException.Message,
                        error = new
                        {
                            code = validatorException.ErrorCode,
                            type = errorType,
                        },
                        traceId,
                        timestamp = validatorException.Timestamp
                    };
                }
            }
            else if (ex is BaseCustomException baseCustomException)
            {
                statusCode = (int)baseCustomException.HttpStatusCode;

                _logger.LogWarning(ex,
                    "Custom error occurred. " +
                    "TraceId: {TraceId}, " +
                    "StatusCode: {StatusCode}, " +
                    "ErrorCode: {ErrorCode}, " +
                    "ErrorType: {ErrorType}, " +
                    "Message: {Message}, " +
                    "RequestPath: {RequestPath}, " +
                    "RequestMethod: {RequestMethod}, " +
                    "Timestamp: {Timestamp}",
                    traceId,
                    statusCode,
                    baseCustomException.ErrorCode,
                    errorType,
                    baseCustomException.Message,
                    requestPath,
                    requestMethod,
                    baseCustomException.Timestamp);

                response = new
                {
                    success = false,
                    message = baseCustomException.Message,
                    error = new
                    {
                        code = baseCustomException.ErrorCode,
                        type = errorType
                    },
                    traceId,
                    timestamp = baseCustomException.Timestamp
                };
            }
            else
            {
                statusCode = 500;

                _logger.LogError(ex,
                    "System error occurred. " +
                    "TraceId: {TraceId}, " +
                    "StatusCode: {StatusCode}, " +
                    "ErrorType: {ErrorType}, " +
                    "Message: {Message}, " +
                    "RequestPath: {RequestPath}, " +
                    "RequestMethod: {RequestMethod}, " +
                    "StackTrace: {StackTrace}",
                    traceId,
                    statusCode,
                    errorType,
                    ex.Message,
                    requestPath,
                    requestMethod,
                    ex.StackTrace);

                response = new
                {
                    success = false,
                    message = ErrorMessages.SystemError,
                    error = new
                    {
                        code = "SYSTEM_ERROR",
                        type = errorType
                    },
                    traceId,
                    timestamp = DateTime.UtcNow
                };
            }

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response, _responseJsonOptions));
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}