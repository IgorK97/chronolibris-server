using MediatR;
using Chronolibris.Domain.Models;

namespace Chronolibris.Application.Requests.References
{
    public record GetAllCountriesQuery() : IRequest<IEnumerable<CountryDto>> { }

    public record GetCountryByIdQuery(long Id) : IRequest<CountryDto?>;

    public record CreateCountryCommand(string Name) : IRequest<long>;

    public record UpdateCountryCommand(long Id, string Name) : IRequest<bool>;

    public record DeleteCountryCommand(long Id) : IRequest<bool>;
}