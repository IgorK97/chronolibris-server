using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Exceptions
{
    public enum ErrorType
    {
        Validation = 400,
        Forbidden = 403,
        NotFound = 404,
        Conflict = 409,
        Unprocessable = 422,
        TooManyRequests = 429,
        ServerException = 500,
    }
    public class ChronolibrisException : Exception
    {
        public ErrorType ErrorType { get; }
        public ChronolibrisException(string message, 
            ErrorType errorType) : base(message)
        {
            ErrorType = errorType;
        }
    }
}
