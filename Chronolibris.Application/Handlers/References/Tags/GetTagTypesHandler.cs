using Chronolibris.Application.Requests.References.Tags;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.References.Tags
{
    public class GetTagTypesHandler : IRequestHandler<GetTagTypesQuery, List<TagType>>
    {
        private readonly ITagsRepository _tagsRepository;
        public GetTagTypesHandler(ITagsRepository tagsRepository)
        {
            _tagsRepository = tagsRepository;
        }
        public async Task<List<TagType>> Handle(GetTagTypesQuery query, CancellationToken cancellationToken)
        {
            return await _tagsRepository.GetTagTypesAsync(cancellationToken);

        }
    }
}
