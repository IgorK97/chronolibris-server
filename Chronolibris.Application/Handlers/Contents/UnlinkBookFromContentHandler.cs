using Chronolibris.Application.Requests.Contents;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.Contents
{

    public class UnlinkBookFromContentHandler : IRequestHandler<UnlinkBookFromContentCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UnlinkBookFromContentHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UnlinkBookFromContentCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.Contents.UnlinkContentFromBookAsync(
                request.ContentId, request.BookId, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
