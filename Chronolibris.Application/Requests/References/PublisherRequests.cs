using MediatR;
using Chronolibris.Application.Models;
using System.Collections.Generic;

namespace Chronolibris.Application.Requests.References
{
    public record GetAllPublishersQuery() : IRequest<IEnumerable<PublisherDto>> { }

    public record GetPublisherByIdQuery(long Id) : IRequest<PublisherDto?>;

    public record CreatePublisherCommand(string Name, string Description, long CountryId) : IRequest<long>;

    public record UpdatePublisherCommand(long Id, string Name, string Description, long CountryId) : IRequest<bool>;

    public record DeletePublisherCommand(long Id) : IRequest<bool>;
}