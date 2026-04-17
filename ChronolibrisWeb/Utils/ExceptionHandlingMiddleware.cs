using System.Diagnostics;
using Chronolibris.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChronolibrisWeb.Middleware
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
        public async Task InvokeAsync(HttpContext context, IExceptionMapper mapper)
        {

            try
            {
                _logger.LogInformation(" {Method} {Path} | Запрос: {Query} | Пользователь: {User}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Request.QueryString,
                    context.User.Identity?.Name ?? "anonymous");

                var sw = Stopwatch.StartNew();
                await _next(context);

                sw.Stop();

                _logger.LogInformation(" {Method} {Path} {StatusCode} ({Elapsed}ms)",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    sw.ElapsedMilliseconds);
            }

            catch (Exception exception)
            {
                _logger.LogError(
                     exception,
                    "Ошибка{Method} {Path}." +
                    " Запрос: {QueryString}. Сообщение: {Message}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Request.QueryString,
                    exception.Message);

                if (context.Response.HasStarted)
                    return; //если уже что-то начало передаваться клиенту, то лучше не записывать

                var (statusCode, title, detail) = mapper.Map(exception);

                context.Response.StatusCode = statusCode;

                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = statusCode,
                    Detail = detail,
                    Instance = context.Request.Path
                });
            }
        }


    }
}
