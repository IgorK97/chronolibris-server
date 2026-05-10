using Chronolibris.Application.Requests.Contents;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.Contents
{
    public class LinkBookToContentHandler : IRequestHandler<LinkBookToContentCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public LinkBookToContentHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(LinkBookToContentCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.Contents.LinkContentToBookAsync(
                request.ContentId, request.BookId, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
