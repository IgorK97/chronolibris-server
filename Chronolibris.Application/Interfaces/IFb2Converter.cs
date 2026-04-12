using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Models;

namespace Chronolibris.Application.Interfaces
{

    public interface IFb2Converter
    {
        Task<ConversionResult> ConvertAsync(
            Stream fb2Stream,
            long? bookId = null,
            ConversionOptions? options = null,
            CancellationToken cancellationToken = default);
    }
}
