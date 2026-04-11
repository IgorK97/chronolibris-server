using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests.Shelves;
using Chronolibris.Domain.Interfaces;
using MediatR;

namespace Chronolibris.Application.Handlers.Shelves
{
    public class AddBookToShelfHandler : IRequestHandler<AddBookToShelfCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        public AddBookToShelfHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<bool> Handle(AddBookToShelfCommand request, CancellationToken ct)
        {
            // Вызов метода репозитория для добавления книги на полку
            await _uow.Shelves.AddBookToShelf(request.ShelfId, request.BookId, ct);
            // Сохранение всех ожидающих изменений в базе данных
            await _uow.SaveChangesAsync(ct);
            return true;
        }
    }

}
