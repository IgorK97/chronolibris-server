using Chronolibris.Application.Requests.Contents;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.Contents
{
    public class GetContentsHandler : IRequestHandler<GetContentsQuery, PagedResult<ContentDto>>
    {
        private readonly IContentRepository _contentRepository;

        public GetContentsHandler(IContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
        }

        public async Task<PagedResult<ContentDto>> Handle(GetContentsQuery request, CancellationToken cancellationToken)
        {
            return await _contentRepository.GetWithFilterAsync(
                request.Filter, cancellationToken);

        }
    }
}
