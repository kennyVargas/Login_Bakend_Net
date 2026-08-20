using Apitransac.Common;
using Apitransac.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace Apitransac.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            _logger.LogError(
                exception,
                "Se produjo una excepción no controlada.");

            var statusCode = HttpStatusCode.InternalServerError;
            var errorCode = "INTERNAL_SERVER_ERROR";
            var message = "Ocurrió un error interno en el servidor.";

            switch (exception)
            {
                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    errorCode = "UNAUTHORIZED";
                    message = exception.Message;
                    break;

                case ConflictException:
                    statusCode = HttpStatusCode.Conflict;
                    errorCode = "CONFLICT";
                    message = exception.Message;
                    break;
            }

            var response = new ApiResponse<object>
            {
                IsSuccess = false,
                StatusCode = (int)statusCode,
                Data = null,
                Errors =
                [
                    new ApiError
                {
                    Code = errorCode,
                    Message = message
                }
                ]
            };

            context.Response.StatusCode = (int)statusCode;

            context.Response.ContentType =
                "application/json";

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}
