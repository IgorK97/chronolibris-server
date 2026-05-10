using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.Selections;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.Selections
{

    public class GetSelectionsQueryHandler(ISelectionsRepository selectionsRepository)
    : IRequestHandler<GetSelectionsQuery, PagedResult<SelectionDetails>>
    {

        public async Task<PagedResult<SelectionDetails>> Handle(GetSelectionsQuery request, CancellationToken ct)
        {
            var selections = await selectionsRepository.GetSelectionsAsync(request.LastId,
                request.Limit+1, request.OnlyActive, ct);

            var hasMore = selections.Count > request.Limit;
            var pageItems = hasMore ? selections.Take(request.Limit) : selections;

            return new PagedResult<SelectionDetails>
            {
                HasNext = hasMore,
                Items = pageItems,
                LastId = pageItems.LastOrDefault()?.Id,
                Limit = request.Limit,
            };
        }
    }

}
