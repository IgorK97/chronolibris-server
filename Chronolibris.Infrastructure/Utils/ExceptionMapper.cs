using Chronolibris.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Chronolibris.Infrastructure.Utils
{
    public class ExceptionMapper : IExceptionMapper
    {
        public (int StatusCode, string Title, string Detail) Map(Exception exception)
        {
            return exception switch
            {
                //UniqueConstraintException //их как-то маппить нужно, потом доделаю
                ChronolibrisException ex => (MapTypeToStatusCode(ex.ErrorType), "Ошибка", ex.Message),
                _ => (StatusCodes.Status500InternalServerError, "Ошибка сервера", "Произошла непредвиденная ошибка")
            };
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
