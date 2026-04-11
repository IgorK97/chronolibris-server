using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests.Selections;
using Chronolibris.Domain.Interfaces;
using MediatR;

namespace Chronolibris.Application.Handlers.Selections
{
    public class RemoveBookFromSelectionHandler : IRequestHandler<RemoveBookFromSelectionRequest, bool>
    {
        private readonly ISelectionsRepository _repository;

        public RemoveBookFromSelectionHandler(ISelectionsRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(RemoveBookFromSelectionRequest request, CancellationToken ct)
        {
            return await _repository.RemoveBookFromSelectionAsync(
                request.SelectionId,
                request.BookId,
                ct
            );
        }
    }
}
