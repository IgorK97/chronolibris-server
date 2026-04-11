using System.Diagnostics;
using Chronolibris.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChronolibrisPrototype.Middleware
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


            //await _next(context);


            try
            {
                _logger.LogInformation("➡️ {Method} {Path} | Query: {Query} | User: {User}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Request.QueryString,
                    context.User.Identity?.Name ?? "anonymous");

                var sw = Stopwatch.StartNew();
                await _next(context);

                sw.Stop();

                _logger.LogInformation("✅ {Method} {Path} → {StatusCode} ({Elapsed}ms)",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    sw.ElapsedMilliseconds);
            }

            catch (Exception exception)
            {
                _logger.LogError(
                     exception,
                    "Error processing {Method} {Path}." +
                    " Query: {QueryString}. Message: {Message}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Request.QueryString,
                    exception.Message);

                int statusCode;
                string detail;

                if(exception is ChronolibrisException)
                {
                    statusCode = MapTypeToStatusCode((exception as ChronolibrisException).ErrorType);
                    detail = exception.Message;
                }
                else
                {
                    statusCode = StatusCodes.Status500InternalServerError;
                    detail = "Ошибка сервера";
                }

                    context.Response.StatusCode = statusCode;

                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = statusCode,
                    Detail = detail,
                    Instance = context.Request.Path
                });
            }
        }

        private static int MapTypeToStatusCode(ErrorType type) => type switch {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.TooManyRequests => StatusCodes.Status429TooManyRequests,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status400BadRequest
        };
    }
}
