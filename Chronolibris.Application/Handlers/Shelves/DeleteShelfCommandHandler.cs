using Chronolibris.Application.Requests.Shelves;
using Chronolibris.Domain.Interfaces.Repository;
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
            //var shelf = await _uow.Shelves.GetByIdAsync(request.ShelfId, ct);
            //if (shelf == null)
            //    return Unit.Value;

            //if (shelf.UserId != request.UserId)
            //    throw new ChronolibrisException("Нет прав на совершение данной операции", ErrorType.Forbidden);

            await _uow.Shelves.DeleteAsync(shelf => shelf.UserId == request.UserId && shelf.Id == request.ShelfId, ct);
            //await _uow.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
