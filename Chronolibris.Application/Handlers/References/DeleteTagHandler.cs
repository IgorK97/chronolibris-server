using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Interfaces;
using MediatR;

namespace Chronolibris.Application.Handlers.References
{
    public class DeleteTagHandler : IRequestHandler<DeleteTagRequest, bool>
    {
        private readonly ITagsRepository _repository;

        public DeleteTagHandler(ITagsRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteTagRequest request, CancellationToken ct)
        {
            return await _repository.DeleteAsync(request.TagId, ct);
        }
    }
}
