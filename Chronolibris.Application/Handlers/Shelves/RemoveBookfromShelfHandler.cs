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

    public class RemoveBookFromShelfHandler : IRequestHandler<RemoveBookFromShelfCommand>
    {
        private readonly IUnitOfWork _uow;


        public RemoveBookFromShelfHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }


        public async Task Handle(RemoveBookFromShelfCommand request, CancellationToken ct)
        {
            var shelf = await _uow.Shelves.GetByIdAsync(request.ShelfId, ct);
            if (shelf is null || shelf.UserId != request.UserId)
            {
                throw new ChronolibrisException
                    ("Полка не найдена или доступ к ней ограничен",
                    ErrorType.Forbidden);
            }
            await _uow.Shelves.RemoveBookFromShelf(request.ShelfId, request.BookId, ct);
            await _uow.SaveChangesAsync(ct);
        }
    }

}
