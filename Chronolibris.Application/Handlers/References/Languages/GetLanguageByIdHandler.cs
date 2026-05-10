using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.References.Languages
{
    public class GetLanguageByIdHandler : IRequestHandler<GetLanguageByIdQuery, LanguageDto?>
    {
        private readonly IGenericRepository<Language> _repository;

        public GetLanguageByIdHandler(IGenericRepository<Language> repository)
        {
            _repository = repository;
        }

        public async Task<LanguageDto?> Handle(GetLanguageByIdQuery request, CancellationToken cancellationToken)
        {
            var language = await _repository.GetByIdAsync(request.id, cancellationToken);
            if (language == null) return null;

            return new LanguageDto
            {
                Id = language.Id,
                Name = language.Name,
            };
        }
    }
}
