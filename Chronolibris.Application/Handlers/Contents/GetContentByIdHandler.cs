using Chronolibris.Application.Requests.Contents;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.Contents
{
    public class GetContentByIdHandler : IRequestHandler<GetContentByIdQuery, ContentDto?>
    {
        private readonly IContentRepository _contentRepository;

        public GetContentByIdHandler(IContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
        }

        public async Task<ContentDto?> Handle(GetContentByIdQuery request, CancellationToken cancellationToken)
        {
            var contentDto = await _contentRepository.GetDtoByIdAsync(request.Id, cancellationToken);

            return contentDto;
        }
    }
}
