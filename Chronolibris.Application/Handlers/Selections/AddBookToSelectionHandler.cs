using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests.Selections;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.Selections
{
    public class AddBookToSelectionHandler : IRequestHandler<AddBookToSelectionRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddBookToSelectionHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(AddBookToSelectionRequest request, CancellationToken ct)
        {
            var selectionExists = await _unitOfWork.Selections.AnyAsync(s => s.Id == request.SelectionId, ct);
            if (!selectionExists)
            {
                throw new ChronolibrisException("Такая подборка не найдена", ErrorType.NotFound);
            }

            var bookExists = await _unitOfWork.Books.AnyAsync(s => s.Id == request.SelectionId, ct);
            if (!bookExists)
            {
                throw new ChronolibrisException("Такая книга не найдена", ErrorType.NotFound);
            }

            var bookInSelection = await _unitOfWork.Selections.IsBookInSelection(request.BookId,
                request.SelectionId, ct);
            if (bookInSelection)
                return;

            return await _unitOfWork.Selections.AddBookToSelectionAsync(
                request.SelectionId,
                request.BookId,
                ct
            );
        }
    }
}
