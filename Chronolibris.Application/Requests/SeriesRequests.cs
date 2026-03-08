// File: Chronolibris.Application.Requests.SeriesRequests.cs
using MediatR;
using Chronolibris.Application.Models;
using System.Collections.Generic;

namespace Chronolibris.Application.Requests
{
    // Queries
    public class GetAllSeriesQuery : IRequest<IEnumerable<SeriesDto>> { }

    public class GetSeriesByIdQuery : IRequest<SeriesDto?>
    {
        public long Id { get; set; }
        public GetSeriesByIdQuery(long id) => Id = id;
    }

    // Commands
    public class CreateSeriesCommand : IRequest<long>
    {
        public string Name { get; set; } = string.Empty;
        public long PublisherId { get; set; }

        public CreateSeriesCommand(string name, long publisherId)
        {
            Name = name;
            PublisherId = publisherId;
        }
    }

    public class UpdateSeriesCommand : IRequest<bool>
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public long PublisherId { get; set; }

        public UpdateSeriesCommand(long id, string name, long publisherId)
        {
            Id = id;
            Name = name;
            PublisherId = publisherId;
        }
    }

    public class DeleteSeriesCommand : IRequest<bool>
    {
        public long Id { get; set; }
        public DeleteSeriesCommand(long id) => Id = id;
    }
}