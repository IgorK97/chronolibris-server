using MediatR;
using Chronolibris.Application.Models;
using System.Collections.Generic;

namespace Chronolibris.Application.Requests.References
{
    public record GetAllFormatsQuery() : IRequest<IEnumerable<FormatDto>> { }
}