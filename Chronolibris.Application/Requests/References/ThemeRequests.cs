using MediatR;
using Chronolibris.Application.Models;
using System.Collections.Generic;

namespace Chronolibris.Application.Requests.References
{
    public record GetAllThemesQuery(long? ParentThemeId) : IRequest<IEnumerable<ThemeDto>>;

    public record GetThemeByIdQuery(long Id) : IRequest<ThemeDto?>;

    public record GetThemesByNameQuery(string Name) : IRequest<List<ThemeDto>>;

    public record CreateThemeCommand(string Name, long? ParentThemeId) : IRequest<long>;

    public record UpdateThemeCommand(long Id, string Name, long? ParentThemeId) : IRequest<Unit>;

    public record DeleteThemeCommand(long Id) : IRequest<Unit>;
}