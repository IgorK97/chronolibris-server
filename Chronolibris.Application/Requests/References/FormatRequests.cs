using MediatR;
using Chronolibris.Application.Models;
using System.Collections.Generic;

namespace Chronolibris.Application.Requests.References
{
    public record GetAllFormatsQuery() : IRequest<IEnumerable<FormatDto>> { }

    public record GetFormatByIdQuery(long Id) : IRequest<FormatDto?>;

    public record CreateFormatCommand(string Name) : IRequest<int>;

    public record UpdateFormatCommand(long Id, string Name) : IRequest<bool>;

    public record DeleteFormatCommand(long Id) : IRequest<bool>;
}