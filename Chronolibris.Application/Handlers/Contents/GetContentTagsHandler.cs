using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests.Contents;
using Chronolibris.Domain.Interfaces;
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
