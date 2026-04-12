using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.References.Tags;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.References.Tags
{
    public class GetChildTagsHandler : IRequestHandler<GetChildTagsQuery, PagedResult<TagDetails>>
    {
        private readonly ITagsRepository _tagsRepository;
        public GetChildTagsHandler(ITagsRepository tagsRepository)
        {
            _tagsRepository = tagsRepository;
        }
        public async Task<PagedResult<TagDetails>> Handle(GetChildTagsQuery query, CancellationToken cancellationToken)
        {
            var items = await _tagsRepository.GetChildTagsAsync(
                 query.parentId, query.lastId, query.pageSize, cancellationToken);

            var hasNext = items.Count > query.pageSize;
            var result = items.Take(query.pageSize).ToList();

            return new PagedResult<TagDetails>
            {
                Items = result,
                Limit = query.pageSize,
                HasNext = hasNext,
                LastId = result.LastOrDefault()?.Id
            };
        }
    }
}
