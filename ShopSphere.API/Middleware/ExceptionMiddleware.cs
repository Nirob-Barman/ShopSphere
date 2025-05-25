using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security;
using System.Text.Json;

namespace ShopSphere.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
                _logger.LogError(ex, "Unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            //context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var statusCode = exception switch
            {
                // 400 Bad Request - Client-side input problems
                ArgumentNullException => (int)HttpStatusCode.BadRequest,
                ArgumentException => (int)HttpStatusCode.BadRequest,
                FormatException => (int)HttpStatusCode.BadRequest,
                InvalidCastException => (int)HttpStatusCode.BadRequest,
                JsonException => (int)HttpStatusCode.BadRequest,
                IndexOutOfRangeException => (int)HttpStatusCode.BadRequest,
                OverflowException => (int)HttpStatusCode.BadRequest, // Too large/small numeric values
                DivideByZeroException => (int)HttpStatusCode.BadRequest,

                // 401 Unauthorized - Missing or invalid authentication
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,

                // 403 Forbidden - Authenticated but not allowed
                SecurityException => (int)HttpStatusCode.Forbidden,

                // 404 Not Found - Resource doesn't exist
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                FileNotFoundException => (int)HttpStatusCode.NotFound,
                DirectoryNotFoundException => (int)HttpStatusCode.NotFound,

                // 405 Method Not Allowed (optional) - Unsupported HTTP method
                NotSupportedException => (int)HttpStatusCode.MethodNotAllowed,

                // 408 Request Timeout - Client waited too long
                TimeoutException => (int)HttpStatusCode.RequestTimeout,

                // 409 Conflict - State conflict (e.g., duplicate, invalid update)
                InvalidOperationException => (int)HttpStatusCode.Conflict,


                // 422 Unprocessable Entity (common for validation) - Validation errors
                ValidationException => 422, // FluentValidation, for example

                // 429 Too Many Requests - Rate limiting or cancellation
                OperationCanceledException => 429, // Optional, e.g. cancellation token use

                // 500 Internal Server Error - Catch-all, unexpected server issues
                StackOverflowException => (int)HttpStatusCode.InternalServerError,
                OutOfMemoryException => (int)HttpStatusCode.InternalServerError,
                AccessViolationException => (int)HttpStatusCode.InternalServerError,
                AppDomainUnloadedException => (int)HttpStatusCode.InternalServerError,

                // 501 Not Implemented - Feature not supported
                NotImplementedException => (int)HttpStatusCode.NotImplemented,
                               
                // Fallback
                _ => (int)HttpStatusCode.InternalServerError
            };

            var response = new
            {
                //StatusCode = context.Response.StatusCode,
                StatusCode = statusCode,
                Message = "An unexpected error occurred.",
                Details = exception.Message // Optional: hide in production
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
