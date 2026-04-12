using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Models.Search;

namespace Chronolibris.Domain.Interfaces.Repository
{
    public interface IReferenceSearchRepository
    {
        Task<List<LanguageDto>> GetAllLanguagesAsync(CancellationToken ct = default);

        Task<List<CountryDto>> GetAllCountriesAsync(CancellationToken ct = default);

        Task<List<PersonRoleDto>> GetAllPersonRolesAsync(CancellationToken ct = default);
        Task<List<PersonSuggestionDto>> SearchPersonsAsync(
            string name, int limit = 10, CancellationToken ct = default);
        Task<List<TagSuggestionDto>> SearchTagsAsync(
            string name, int limit = 10, CancellationToken ct = default);

        Task<List<PersonSuggestionDto>> GetPersonsByIdsAsync(List<long> ids, CancellationToken ct = default);
        Task<List<TagSuggestionDto>> GetTagsByIdsAsync(List<long> ids, CancellationToken ct = default);
    }

}
