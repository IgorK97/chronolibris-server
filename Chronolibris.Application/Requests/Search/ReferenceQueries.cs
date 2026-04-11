using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Models.Search;
using MediatR;

namespace Chronolibris.Application.Requests.Search
{
    public record GetLanguagesQuery : IRequest<List<LanguageDto>>;

    public record GetCountriesQuery : IRequest<List<CountryDto>>;

    public record GetPersonRolesQuery : IRequest<List<PersonRoleDto>>;

    public record SearchPersonsQuery(string Name, int Limit = 10)
        : IRequest<List<PersonSuggestionDto>>;

    public record SearchTagsQuery(string Name, int Limit = 10)
        : IRequest<List<TagSuggestionDto>>;

    public record GetPersonsByIdsQuery(List<long> Ids):IRequest<List<PersonSuggestionDto>>;

    public record GetTagsByIdsQuery(List<long> Ids):IRequest<List<TagSuggestionDto>>;

}
