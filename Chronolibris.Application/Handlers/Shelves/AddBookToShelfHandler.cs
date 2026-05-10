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
            var shelfOwnerValid = await _uow.Shelves.AnyAsync(s =>
                    s.Id == request.ShelfId && s.UserId == request.UserId, ct);

            if (!shelfOwnerValid)
                throw new ChronolibrisException("Полка не найдена", ErrorType.Forbidden);

            var bookAvailable = await _uow.Books.AnyAsync(b =>
                b.Id == request.BookId && b.IsAvailable, ct);

            if (!bookAvailable) //так проверять не критично, добавил ведь уже ограничение на получение книг на полке
                //если книга скрыта, то не будет видна нигде
                throw new ChronolibrisException("Книга недоступна", ErrorType.Forbidden);

            await _uow.Shelves.AddBookToShelf(request.ShelfId, request.BookId, ct);
            //await _uow.SaveChangesAsync(ct);
        }
    }
}