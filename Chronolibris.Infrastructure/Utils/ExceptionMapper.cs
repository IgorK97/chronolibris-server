using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Exceptions;
using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Chronolibris.Infrastructure.Utils
{
    public class ExceptionMapper : IExceptionMapper
    {
        public (int StatusCode, string Title, string Detail) Map(Exception exception)
        {
            return exception switch
            {
                ChronolibrisException ex => (MapTypeToStatusCode(ex.ErrorType), "Ошибка", ex.Message),
                CannotInsertNullException ex => (StatusCodes.Status400BadRequest, "Ошибка", "Некоторые обязательные данные не были указаны"),


                //DbUpdateException dbEx when dbEx.InnerException?.Message.Contains("duplicate key") == true =>
                    UniqueConstraintException ex=>(StatusCodes.Status409Conflict, "Ошибка", FormatUniqueMessage(ex.Entries)),

                TaskCanceledException => (StatusCodes.Status504GatewayTimeout, "Превышено время ожидания", "Превышено время ожидания"),
                
                _ => (StatusCodes.Status500InternalServerError, "Ошибка сервера", "Произошла непредвиденная ошибка")
            };
        }

        private string FormatUniqueMessage(IReadOnlyList<EntityEntry> entries)
        {
            var details = entries.Select(entry =>
            {
                var entityName = entry.Metadata.GetComment() ?? entry.Entity.GetType().Name;

                var properties = entry.CurrentValues.Properties
                .Where(p => p.IsIndex() || p.IsPrimaryKey())
                .Select(p => $"{p.GetComment() ?? p.Name}: '{entry.CurrentValues[p]}");

                return $"{entityName} ({string.Join(", ", properties)})";
            });
            return $"Ошибка. Указанные поля должны быть уникальными в системе, но таковыми не являются: {string.Join("; ", details)}";
        }

        private static int MapTypeToStatusCode(ErrorType type) => type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.TooManyRequests => StatusCodes.Status429TooManyRequests,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unprocessable => StatusCodes.Status422UnprocessableEntity,
            ErrorType.ServerException => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status400BadRequest
        };
    }
}
