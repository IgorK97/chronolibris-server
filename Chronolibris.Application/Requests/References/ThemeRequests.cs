using MediatR;
using Chronolibris.Application.Models;
using System.Collections.Generic;

namespace Chronolibris.Application.Requests.References
{
    public class GetAllThemesQuery : IRequest<IEnumerable<ThemeDto>>
    {
        public long? ParentThemeId { get; set; }

        public GetAllThemesQuery(long? parentThemeId = null)
        {
            ParentThemeId = parentThemeId;
        }
    }

    public class GetThemeByIdQuery : IRequest<ThemeDto?>
    {
        public long Id { get; set; }
        public GetThemeByIdQuery(long id) => Id = id;
    }

    public record GetThemesByNameQuery(string Name) : IRequest<List<ThemeDto>>;

    public class CreateThemeCommand : IRequest<long>
    {
        public string Name { get; set; } = string.Empty;
        public long? ParentThemeId { get; set; }

        public CreateThemeCommand(string name, long? parentThemeId)
        {
            Name = name;
            ParentThemeId = parentThemeId;
        }
    }

    public class UpdateThemeCommand : IRequest<Unit>
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public long? ParentThemeId { get; set; }

        public UpdateThemeCommand(long id, string name, long? parentThemeId)
        {
            Id = id;
            Name = name;
            ParentThemeId = parentThemeId;
        }
    }

    public class DeleteThemeCommand : IRequest<Unit>
    {
        public long Id { get; set; }
        public DeleteThemeCommand(long id) => Id = id;
    }
}