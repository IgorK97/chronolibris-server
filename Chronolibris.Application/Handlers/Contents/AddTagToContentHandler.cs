using Chronolibris.Application.Requests.Contents;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.Contents
{
    public class AddTagToContentHandler : IRequestHandler<AddTagToContentCommand, bool>
    {
        private readonly IContentRepository _repository;

        public AddTagToContentHandler(IContentRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(AddTagToContentCommand request, CancellationToken ct)
        {
            return await _repository.AddTagAsync(request.ContentId, request.TagId, ct);
        }
    }
}
