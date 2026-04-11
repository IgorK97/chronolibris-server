using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.References
{
    public class GetTagsHandler : IRequestHandler<GetTagsQuery, PagedResult<TagDetails>>
    {
        private readonly ITagsRepository _repository;

        public GetTagsHandler(ITagsRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<TagDetails>> Handle(GetTagsQuery request, CancellationToken ct)
        {
            var tags = await _repository.GetTagsAsync(
         request.TagTypeId,
         request.SearchTerm,
         request.LastId,
         request.Limit,
         ct
     );

            var hasNext = tags.Count > request.Limit;

            if (hasNext)
                tags.RemoveAt(tags.Count - 1);

            return new PagedResult<TagDetails>
            {
                Items = tags,
                Limit = request.Limit,
                HasNext = hasNext,
                LastId = tags.LastOrDefault()?.Id
            };
        }
    }
}
