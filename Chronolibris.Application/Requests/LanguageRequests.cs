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
        public string FtsConfiguration { get; set; } = "russian";

        public CreateLanguageCommand(string name, string ftsConfiguration)
        {
            Name = name;
            FtsConfiguration = ftsConfiguration;
        }
    }

    public class UpdateLanguageCommand : IRequest<bool>
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FtsConfiguration { get; set; } = "russian";

        public UpdateLanguageCommand(long id, string name, string ftsConfiguration)
        {
            Id = id;
            Name = name;
            FtsConfiguration = ftsConfiguration;
        }
    }

    public class DeleteLanguageCommand : IRequest<bool>
    {
        public long Id { get; set; }
        public DeleteLanguageCommand(long id) => Id = id;
    }
}