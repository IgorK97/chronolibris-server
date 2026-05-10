using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.References.Languages
{
    public class GetAllLanguagesHandler : IRequestHandler<GetAllLanguagesQuery, IEnumerable<LanguageDto>>
    {
        private readonly ISearchRepository _repository;

        public GetAllLanguagesHandler(ISearchRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<LanguageDto>> Handle(GetAllLanguagesQuery request, CancellationToken cancellationToken)
        {
            var languages = await _repository.GetAllLanguagesAsync(cancellationToken);
            return languages.Select(l => new LanguageDto
            {
                Id = l.Id,
                Name = l.Name,
            });
        }
    }
}
