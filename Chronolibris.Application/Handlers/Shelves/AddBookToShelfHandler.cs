using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests.Shelves;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.Shelves
{
    public class AddBookToShelfHandler : IRequestHandler<AddBookToShelfCommand>
    {
        private readonly IUnitOfWork _uow;
        public AddBookToShelfHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task Handle(AddBookToShelfCommand request, CancellationToken ct)
        {
            var shelf = await _uow.Shelves.GetByIdAsync(request.ShelfId, ct);
            if(shelf==null || shelf.UserId != request.UserId)
                throw new ChronolibrisException
                    ("Полка не найдена или доступ к ней ограничен", ErrorType.Forbidden);

            var book = await _uow.Books.GetByIdAsync(request.BookId, ct);
            if (book == null || !book.IsAvailable)
                throw new ChronolibrisException
                    ("Книга не найдена или доступ к ней ограничен", ErrorType.Forbidden);

            var alreadyOnShelf = await _uow.Shelves.IsInShelf(request.BookId, request.ShelfId);
            if (alreadyOnShelf)
                //throw new ChronolibrisException("Книга уже на полке", ErrorType.Conflict);
                return;

            await _uow.Shelves.AddBookToShelf(request.ShelfId, request.BookId, ct);
            await _uow.SaveChangesAsync(ct);
        }
    }

}
