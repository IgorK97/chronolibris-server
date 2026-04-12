using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests.Search;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models.Search;
using MediatR;

namespace Chronolibris.Application.Handlers.Search
{
    public class GetLanguagesHandler : IRequestHandler<GetLanguagesQuery, List<LanguageDto>>
    {
        private readonly IReferenceSearchRepository _repo;
        public GetLanguagesHandler(IReferenceSearchRepository repo) => _repo = repo;
        public Task<List<LanguageDto>> Handle(GetLanguagesQuery _, CancellationToken ct)
            => _repo.GetAllLanguagesAsync(ct);
    }

    public class GetCountriesHandler : IRequestHandler<GetCountriesQuery, List<CountryDto>>
    {
        private readonly IReferenceSearchRepository _repo;
        public GetCountriesHandler(IReferenceSearchRepository repo) => _repo = repo;
        public Task<List<CountryDto>> Handle(GetCountriesQuery _, CancellationToken ct)
            => _repo.GetAllCountriesAsync(ct);
    }

    public class GetPersonRolesHandler : IRequestHandler<GetPersonRolesQuery, List<PersonRoleDto>>
    {
        private readonly IReferenceSearchRepository _repo;
        public GetPersonRolesHandler(IReferenceSearchRepository repo) => _repo = repo;
        public Task<List<PersonRoleDto>> Handle(GetPersonRolesQuery _, CancellationToken ct)
            => _repo.GetAllPersonRolesAsync(ct);
    }

    public class SearchPersonsHandler
        : IRequestHandler<SearchPersonsQuery, List<PersonSuggestionDto>>
    {
        private readonly IReferenceSearchRepository _repo;
        public SearchPersonsHandler(IReferenceSearchRepository repo) => _repo = repo;
        public Task<List<PersonSuggestionDto>> Handle(
            SearchPersonsQuery request, CancellationToken ct)
            => _repo.SearchPersonsAsync(request.Name, request.Limit, ct);
    }

    public class SearchTagsHandler
        : IRequestHandler<SearchTagsQuery, List<TagSuggestionDto>>
    {
        private readonly IReferenceSearchRepository _repo;
        public SearchTagsHandler(IReferenceSearchRepository repo) => _repo = repo;
        public Task<List<TagSuggestionDto>> Handle(
            SearchTagsQuery request, CancellationToken ct)
            => _repo.SearchTagsAsync(request.Name, request.Limit, ct);
    }

    public class GetPersonsByIdsHandler : IRequestHandler<GetPersonsByIdsQuery,
        List<PersonSuggestionDto>>
    {
        private readonly IReferenceSearchRepository _repo;
        public GetPersonsByIdsHandler(IReferenceSearchRepository repository) => _repo = repository;

        public Task<List<PersonSuggestionDto>> Handle(
            GetPersonsByIdsQuery request, CancellationToken ct)
            => _repo.GetPersonsByIdsAsync(request.Ids, ct);
    }

    public class GetTagsByIdsHandler : IRequestHandler<GetTagsByIdsQuery, List<TagSuggestionDto>>
    {
        private readonly IReferenceSearchRepository _repo;
        public GetTagsByIdsHandler(IReferenceSearchRepository repo) => _repo = repo;

        public Task<List<TagSuggestionDto>> Handle(
            GetTagsByIdsQuery request, CancellationToken ct)
            => _repo.GetTagsByIdsAsync(request.Ids, ct);
    }
}
