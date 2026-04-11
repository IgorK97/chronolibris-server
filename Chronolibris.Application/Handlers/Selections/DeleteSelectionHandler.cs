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
    public class DeleteSelectionHandler : IRequestHandler<DeleteSelectionRequest, bool>
    {
        private readonly ISelectionsRepository _repository;

        public DeleteSelectionHandler(ISelectionsRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteSelectionRequest request, CancellationToken ct)
        {
            return await _repository.DeleteAsync(request.SelectionId, ct);
        }
    }
}
