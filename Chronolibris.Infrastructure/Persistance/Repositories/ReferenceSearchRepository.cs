using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models.Search;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.DataAccess.Persistance.Repositories
{
    public class ReferenceSearchRepository : IReferenceSearchRepository
    {
        private readonly ApplicationDbContext _context;
        private const int SynonymRelationTypeId = 1;

        public ReferenceSearchRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<PersonSuggestionDto>> GetPersonsByIdsAsync(
    List<long> ids, CancellationToken ct = default)
        {
            return _context.Persons
                .AsNoTracking()
                .Where(p => ids.Contains(p.Id))
                .Select(p => new PersonSuggestionDto { Id = (int)p.Id, Name = p.Name })
                .ToListAsync(ct);
        }

        public async Task<List<TagSuggestionDto>> GetTagsByIdsAsync(
    List<long> ids, CancellationToken ct = default)
        {
            var tags = await _context.Tags
                .AsNoTracking()
                .Include(t => t.ParentTag)
                .Where(t => ids.Contains(t.Id))
                .ToListAsync(ct);

            var results = new List<TagSuggestionDto>(tags.Count);
            var seenIds = new HashSet<long>();

            foreach (var tag in tags)
            {
                bool isSynonym = tag.RelationTypeId == SynonymRelationTypeId
                                 && tag.ParentTagId.HasValue
                                 && tag.ParentTag is not null;

                var rootTag = isSynonym ? tag.ParentTag! : tag;
                var matchedName = isSynonym ? tag.Name : (string?)null;

                if (!seenIds.Add(rootTag.Id)) continue;

                results.Add(new TagSuggestionDto
                {
                    Id = (int)rootTag.Id,
                    Name = rootTag.Name,
                    MatchedName = matchedName,
                });
            }

            return results;
        }


        public Task<List<LanguageDto>> GetAllLanguagesAsync(CancellationToken ct = default)
        {
            return _context.Languages
                .AsNoTracking()
                .OrderBy(l => l.Name)
                .Select(l => new LanguageDto { Id = l.Id, Name = l.Name })
                .ToListAsync(ct);
        }

        public Task<List<CountryDto>> GetAllCountriesAsync(CancellationToken ct = default)
        {
            return _context.Countries
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CountryDto { Id = c.Id, Name = c.Name })
                .ToListAsync(ct);
        }

        public async Task<List<PersonRoleDto>> GetAllPersonRolesAsync(CancellationToken ct = default)
        {
            var res =  await _context.PersonRoles
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .Select(r => new { Id = r.Id, Name = r.Name, Kind = r.Kind })
                .ToListAsync(ct);
            return res.Select(pr => new PersonRoleDto { Kind = (int)pr.Kind, Id = pr.Id, Name = pr.Name }).ToList();
        }

        public Task<List<PersonSuggestionDto>> SearchPersonsAsync(
            string name, int limit = 10, CancellationToken ct = default)
        {
            var pattern = $"%{name.Trim()}%";

            return _context.Persons
                .AsNoTracking()
                .Where(p => EF.Functions.ILike(p.Name, pattern))
                .OrderBy(p => p.Name)
                .Take(limit)
                .Select(p => new PersonSuggestionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    //ImagePath = p.ImagePath,
                })
                .ToListAsync(ct);
        }

        public async Task<List<TagSuggestionDto>> SearchTagsAsync(
            string name, int limit = 10, CancellationToken ct = default)
        {
            var pattern = $"%{name.Trim()}%";


            var found = await _context.Tags
                .AsNoTracking()
                .Include(t => t.ParentTag)
                .Where(t => EF.Functions.ILike(t.Name, pattern))
                .Take(limit * 2)
                .ToListAsync(ct);

            var results = new List<TagSuggestionDto>();
            var seenIds = new HashSet<long>();

            foreach (var tag in found)
            {
                bool isSynonym = tag.RelationTypeId == SynonymRelationTypeId
                                 && tag.ParentTagId.HasValue
                                 && tag.ParentTag is not null;

                var rootTag = isSynonym ? tag.ParentTag! : tag;
                var matchedName = isSynonym ? tag.Name : (string?)null;
                if (!seenIds.Add(rootTag.Id))
                    continue;

                results.Add(new TagSuggestionDto
                {
                    Id = rootTag.Id,
                    Name = rootTag.Name,
                    MatchedName = matchedName,
                });

                if (results.Count >= limit)
                    break;
            }

            return results;
        }
    }

}
