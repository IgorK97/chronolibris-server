using MediatR;
using Chronolibris.Application.Models;

namespace Chronolibris.Application.Requests.References
{
    public record GetAllFormatsQuery() : IRequest<IEnumerable<FormatDto>>;
}