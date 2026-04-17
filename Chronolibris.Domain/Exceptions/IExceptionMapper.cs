using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Exceptions
{
    public interface IExceptionMapper
    {
        (int StatusCode, string Title, string Detail) Map(Exception exception);
    }
}
