using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests.Contents;
using Chronolibris.Domain.Interfaces;
using MediatR;

namespace Chronolibris.Application.Handlers.Contents
{
    public class RemoveTagFromContentHandler : IRequestHandler<RemoveTagFromContentCommand, bool>
    {
        private readonly IContentRepository _repository;

        public RemoveTagFromContentHandler(IContentRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(RemoveTagFromContentCommand request, CancellationToken ct)
        {
            return await _repository.RemoveTagAsync(request.ContentId, request.TagId, ct);
        }
    }
}
