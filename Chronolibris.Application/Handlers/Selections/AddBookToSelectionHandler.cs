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
    public class AddBookToSelectionHandler : IRequestHandler<AddBookToSelectionRequest, bool>
    {
        private readonly ISelectionsRepository _repository;

        public AddBookToSelectionHandler(ISelectionsRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(AddBookToSelectionRequest request, CancellationToken ct)
        {
            return await _repository.AddBookToSelectionAsync(
                request.SelectionId,
                request.BookId,
                ct
            );
        }
    }
}
