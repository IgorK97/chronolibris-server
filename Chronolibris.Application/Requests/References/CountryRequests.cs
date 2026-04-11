using MediatR;
using Chronolibris.Application.Models;
using System.Collections.Generic;

namespace Chronolibris.Application.Requests.References
{
    public class GetAllCountriesQuery : IRequest<IEnumerable<CountryDto>> { }

    public class GetCountryByIdQuery : IRequest<CountryDto?>
    {
        public long Id { get; set; }
        public GetCountryByIdQuery(long id) => Id = id;
    }

    public class CreateCountryCommand : IRequest<long>
    {
        public string Name { get; set; } = string.Empty;

        public CreateCountryCommand(string name)
        {
            Name = name;
        }
    }

    public class UpdateCountryCommand : IRequest<bool>
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public UpdateCountryCommand(long id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class DeleteCountryCommand : IRequest<bool>
    {
        public long Id { get; set; }
        public DeleteCountryCommand(long id) => Id = id;
    }
}