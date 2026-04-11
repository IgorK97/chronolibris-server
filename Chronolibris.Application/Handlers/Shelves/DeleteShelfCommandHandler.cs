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
    public class DeleteShelfCommandHandler : IRequestHandler<DeleteShelfCommand, Unit>
    {
        private readonly IUnitOfWork _uow;
        public DeleteShelfCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<Unit> Handle(DeleteShelfCommand request, CancellationToken ct)
        {
            var shelf = await _uow.Shelves.GetByIdAsync(request.ShelfId, ct);
            if (shelf == null || shelf.UserId != request.UserId)
                return Unit.Value;
            
            _uow.Shelves.Delete(shelf);
            await _uow.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
