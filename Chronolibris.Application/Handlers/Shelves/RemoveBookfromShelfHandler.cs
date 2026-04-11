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

    public class RemoveBookFromShelfHandler : IRequestHandler<RemoveBookFromShelfCommand, bool>
    {
        private readonly IUnitOfWork _uow;


        public RemoveBookFromShelfHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }


        public async Task<bool> Handle(RemoveBookFromShelfCommand request, CancellationToken ct)
        {
            await _uow.Shelves.RemoveBookFromShelf(request.ShelfId, request.BookId, ct);
            await _uow.SaveChangesAsync(ct);
            return true;
        }
    }

}
