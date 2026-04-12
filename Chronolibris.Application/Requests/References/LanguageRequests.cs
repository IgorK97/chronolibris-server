using MediatR;
using Chronolibris.Application.Models;
using System.Collections.Generic;
using Chronolibris.Domain.Models;

namespace Chronolibris.Application.Requests.References
{
    public record GetAllLanguagesQuery : IRequest<IEnumerable<LanguageDto>> { }

    //public record GetFtsConfigurationsQuery : IRequest<IEnumerable<FtsConfigurationDto>> { }

    public record GetLanguageByIdQuery(long id) : IRequest<LanguageDto?>;

    public record CreateLanguageCommand(string Name) : IRequest<long>;

    public record UpdateLanguageCommand(long Id, string Name) : IRequest<bool>;

    public record DeleteLanguageCommand(long id) : IRequest<bool>;
}