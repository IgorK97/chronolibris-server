using Chronolibris.Application.Requests.References.Tags;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.References.Tags
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
