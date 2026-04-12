using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests.Selections;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.Selections
{
    public class CreateSelectionHandler : IRequestHandler<CreateSelectionRequest, long>
    {
        private readonly ISelectionsRepository _repository;

        public CreateSelectionHandler(ISelectionsRepository repository)
        {
            _repository = repository;
        }

        public async Task<long> Handle(CreateSelectionRequest request, CancellationToken ct)
        {
            var selection = new Selection
            {
                Id = 0, // Auto-generated
                Name = request.Name,
                Description = request.Description,
                UserId = request.UserId,
                //SelectionTypeId = request.SelectionTypeId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            return await _repository.CreateAsync(selection, ct);
        }
    }
}
