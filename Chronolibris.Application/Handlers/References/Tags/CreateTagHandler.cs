using Chronolibris.Application.Requests.References.Tags;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.References.Tags
{
    public class CreateTagHandler : IRequestHandler<CreateTagRequest, long>
    {
        private readonly ITagsRepository _repository;

        public CreateTagHandler(ITagsRepository repository)
        {
            _repository = repository;
        }

        public async Task<long> Handle(CreateTagRequest request, CancellationToken ct)
        {

            var tag = new Tag
            {
                Id = 0,
                Name = request.Name.Trim(),
                TagTypeId = request.TagTypeId,
                ParentTagId = request.ParentTagId,
                RelationTypeId = request.RelationTypeId,
            };

            return await _repository.CreateAsync(tag, ct);
        }
    }
}
