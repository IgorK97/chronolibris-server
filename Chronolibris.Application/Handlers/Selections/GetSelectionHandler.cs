using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.Selections;
using Chronolibris.Domain.Interfaces.Repository;
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
            var selection = await _repository.GetByIdAsync(request.SelectionId, ct);

            if (selection == null)
                return null;

            if ((request.UserId == 0 || !(request.UserRole == "admin" || request.UserRole == "moderator")) && selection?.IsActive == false)
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
