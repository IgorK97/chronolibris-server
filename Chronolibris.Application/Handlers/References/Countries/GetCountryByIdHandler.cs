using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.References.Countries
{

    public class GetCountryByIdHandler : IRequestHandler<GetCountryByIdQuery, CountryDto?>
    {
        private readonly IGenericRepository<Country> _repository;

        public GetCountryByIdHandler(IGenericRepository<Country> repository)
        {
            _repository = repository;
        }

        public async Task<CountryDto?> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
        {
            var country = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (country == null) return null;

            return new CountryDto
            {
                Id = country.Id,
                Name = country.Name
            };
        }
    }
}
