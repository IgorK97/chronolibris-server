using MediatR;
using Chronolibris.Application.Models;
using System.Collections.Generic;

namespace Chronolibris.Application.Requests.References
{
    public class GetAllPublishersQuery : IRequest<IEnumerable<PublisherDto>> { }

    public class GetPublisherByIdQuery : IRequest<PublisherDto?>
    {
        public long Id { get; set; }
        public GetPublisherByIdQuery(long id) => Id = id;
    }

    public class CreatePublisherCommand : IRequest<long>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long CountryId { get; set; }

        public CreatePublisherCommand(string name, string description, long countryId)
        {
            Name = name;
            Description = description;
            CountryId = countryId;
        }
    }

    public class UpdatePublisherCommand : IRequest<bool>
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long CountryId { get; set; }

        public UpdatePublisherCommand(long id, string name, string description, long countryId)
        {
            Id = id;
            Name = name;
            Description = description;
            CountryId = countryId;
        }
    }

    public class DeletePublisherCommand : IRequest<bool>
    {
        public long Id { get; set; }
        public DeletePublisherCommand(long id) => Id = id;
    }
}