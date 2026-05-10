using Chronolibris.Application.Models;
using Chronolibris.Domain.Models;

namespace Chronolibris.Application.Interfaces
{

    public interface IFb2Converter
    {
        Task<ConversionResult> ConvertAsync(
            Stream fb2Stream,
            long bookId,
            ConversionOptions? options = null,
            CancellationToken cancellationToken = default);
    }
}
