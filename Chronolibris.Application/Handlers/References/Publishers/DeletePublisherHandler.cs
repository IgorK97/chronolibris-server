using MediatR;
using Chronolibris.Application.Models;
using Chronolibris.Domain.Entities;
using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Interfaces.Repository;

namespace Chronolibris.Application.Handlers.References.Publishers
{
    public class DeletePublisherHandler : IRequestHandler<DeletePublisherCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeletePublisherHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeletePublisherCommand request, CancellationToken cancellationToken)
        {
            var publisher = await _unitOfWork.Publishers.GetByIdAsync(request.Id, cancellationToken);
            if (publisher == null) return false; //если удалили, надо бы ничего не возвращать, и вообще это идемпотентная ведь операция

            _unitOfWork.Publishers.Delete(publisher);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}