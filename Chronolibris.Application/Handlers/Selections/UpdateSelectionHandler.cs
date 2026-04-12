using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests.Selections;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.Selections
{
    public class UpdateSelectionHandler : IRequestHandler<UpdateSelectionRequest, bool>
    {
        private readonly ISelectionsRepository _repository;

        public UpdateSelectionHandler(ISelectionsRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateSelectionRequest request, CancellationToken ct)
        {
            return await _repository.UpdateAsync(
                request.SelectionId,
                request.Name,
                request.Description,
                request.IsActive,
                ct
            );
        }
    }
}
