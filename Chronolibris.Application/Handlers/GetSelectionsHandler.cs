using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests;
using Chronolibris.Domain.Interfaces;
using MediatR;

namespace Chronolibris.Application.Handlers
{
    public class GetSelectionsHandler : IRequestHandler<GetSelectionsRequest, PagedResult<SelectionDetails>>
    {
        private readonly ISelectionsRepository _repository;

        public GetSelectionsHandler(ISelectionsRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<SelectionDetails>> Handle(GetSelectionsRequest request, CancellationToken ct)
        {
            var selections = await _repository.GetActiveSelectionsAsync(ct);
            var selectionDetails = selections.Select(s => new SelectionDetails
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                //SelectionTypeId = s.SelectionTypeId,
                BooksCount = s.Books.Count
            }).ToList();

            return new PagedResult<SelectionDetails>
            {
                Items = selectionDetails,
                LastId = selectionDetails.Count,
                HasNext = false
            };
        }
    }
}
