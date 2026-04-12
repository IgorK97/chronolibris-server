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
    public class GetRootTagsHandler : IRequestHandler<GetRootTagsQuery, PagedResult<TagDetails>>
    {
        private readonly ITagsRepository _tagsRepository;
        public GetRootTagsHandler(ITagsRepository tagsRepository)
        {
            _tagsRepository = tagsRepository;
        }
        public async Task<PagedResult<TagDetails>> Handle(GetRootTagsQuery query, CancellationToken cancellationToken)
        {
            var items = await _tagsRepository.GetRootTagsAsync(
                 query.tagTypeId, query.searchTerm, query.lastId, query.pageSize, CancellationToken.None);

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
