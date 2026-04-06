// File: Chronolibris.Application.Requests.LanguageRequests.cs
using MediatR;
using Chronolibris.Application.Models;
using System.Collections.Generic;
using Chronolibris.Domain.Models;

namespace Chronolibris.Application.Requests
{
    // Queries
    public class GetAllLanguagesQuery : IRequest<IEnumerable<LanguageDto>> { }

    public class GetFtsConfigurationsQuery : IRequest<IEnumerable<FtsConfigurationDto>> { }

    public class GetLanguageByIdQuery : IRequest<LanguageDto?>
    {
        public long Id { get; set; }
        public GetLanguageByIdQuery(long id) => Id = id;
    }

    // Commands
    public class CreateLanguageCommand : IRequest<long>
    {
        public string Name { get; set; } = string.Empty;

        public CreateLanguageCommand(string name)
        {
            Name = name;
        }
    }

    public class UpdateLanguageCommand : IRequest<bool>
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public UpdateLanguageCommand(long id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class DeleteLanguageCommand : IRequest<bool>
    {
        public long Id { get; set; }
        public DeleteLanguageCommand(long id) => Id = id;
    }
}