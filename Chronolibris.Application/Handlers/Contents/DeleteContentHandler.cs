using Chronolibris.Application.Requests.Contents;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.Contents
{
    public class DeleteContentHandler : IRequestHandler<DeleteContentCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteContentHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteContentCommand request, CancellationToken cancellationToken)
        {
            var content = await _unitOfWork.Contents.GetByIdAsync(request.Id, cancellationToken);
            if (content == null)
                return Unit.Value;

            _unitOfWork.Contents.Delete(content);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
