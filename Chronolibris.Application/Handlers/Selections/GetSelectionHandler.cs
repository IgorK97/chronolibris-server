using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.Selections;
using Chronolibris.Domain.Interfaces;
using MediatR;

namespace Chronolibris.Application.Handlers.Selections
{
    public class GetSelectionHandler : IRequestHandler<GetSelectionQuery, SelectionDetails?>
    {
        private readonly ISelectionsRepository _repository;

        public GetSelectionHandler(ISelectionsRepository repository)
        {
            _repository = repository;
        }

        public async Task<SelectionDetails?> Handle(GetSelectionQuery request, CancellationToken ct)
        {
            var selection = await _repository.GetByIdAsync(request.SelectionId, request.userId, request.userRole, ct);

            if (selection == null)
                return null;

            return new SelectionDetails
            {
                Id = selection.Id,
                Name = selection.Name,
                Description = selection.Description,
                CreatedAt = selection.CreatedAt,
                UpdatedAt = selection.UpdatedAt,
                //SelectionTypeId = selection.SelectionTypeId,
                BooksCount = selection.Books.Count,
                IsActive = selection.IsActive
            };
        }
    }
}
