using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests;
using Chronolibris.Domain.Interfaces;
using MediatR;

namespace Chronolibris.Application.Handlers
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
