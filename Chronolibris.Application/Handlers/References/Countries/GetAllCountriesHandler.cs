using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.References.Countries
{
    public class GetAllCountriesHandler : IRequestHandler<GetAllCountriesQuery, IEnumerable<CountryDto>>
    {
        private readonly IGenericRepository<Country> _repository;

        public GetAllCountriesHandler(IGenericRepository<Country> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CountryDto>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
        {
            var countries = await _repository.GetAllAsync(cancellationToken);
            return countries.Select(c => new CountryDto
            {
                Id = c.Id,
                Name = c.Name
            }).OrderBy(c => c.Name);
        }
    }
}
