using Chronolibris.Application.Requests.Selections;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.Selections
{
    public class SeekBookInSelections : IRequestHandler<SeekBookInSelectionsQuery, List<long>>
    {
        private readonly ISelectionsRepository selectionsRepository;
        public SeekBookInSelections(ISelectionsRepository selRepo) { 
            selectionsRepository = selRepo;
        }
        public Task<List<long>> Handle(SeekBookInSelectionsQuery request, CancellationToken cancellationToken)
        {
            return selectionsRepository.SeekBookInSelections(request.BookId, cancellationToken);
        }
    }
}
