using Chronolibris.Application.Requests.Contents;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.Contents
{
    public class GetContentTagsHandler : IRequestHandler<GetContentTagsQuery, List<TagDetails>>
    {
        private readonly IContentRepository _repository;

        public GetContentTagsHandler(IContentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TagDetails>> Handle(GetContentTagsQuery request, CancellationToken ct)
        {
            return await _repository.GetTagsAsync(request.ContentId, ct);
        }
    }
}
