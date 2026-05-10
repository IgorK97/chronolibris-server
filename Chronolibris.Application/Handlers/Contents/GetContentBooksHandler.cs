using Chronolibris.Application.Requests.Contents;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.Contents
{
    public class GetContentBooksHandler : IRequestHandler<GetContentBooksQuery, List<BookDto>>
    {
        private readonly IContentRepository _contentRepository;

        public GetContentBooksHandler(
            IContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
        }

        public async Task<List<BookDto>> Handle(GetContentBooksQuery request, CancellationToken cancellationToken)
        {
            return await _contentRepository.GetBooksDtoByContentIdAsync(request.ContentId, cancellationToken);
        }
    }
}
