using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using Chronolibris.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.DataAccess.Persistance.Repositories
{
    public class TagsRepository : ITagsRepository
    {
        private readonly ApplicationDbContext _context;

        public TagsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TagType>> GetTagTypesAsync(CancellationToken ct)
        {
            return await _context.TagTypes
                .OrderBy(t => t.Id)
                .ToListAsync(ct);
        }

        public async Task<List<TagDetails>> GetTagsAsync(
            long? tagTypeId,
    string? searchTerm,
    long? lastId,      
    int limit,        
    CancellationToken ct)
        {
            IQueryable<Tag> query = _context.Tags.AsNoTracking()
    .Include(t => t.TagType);

            if (tagTypeId.HasValue)
                query = query.Where(t => t.TagTypeId == tagTypeId.Value);

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(t => EF.Functions.ILike(t.Name, $"%{searchTerm}%"));

            if (lastId.HasValue)
                query = query.Where(t => t.Id > lastId.Value);

            return await query
                .OrderBy(t => t.Id)       
                .Take(limit + 1)          
                .Select(t => new TagDetails
                {
                    Id = t.Id,
                    Name = t.Name,
                    TagTypeId = t.TagTypeId,
                    TagTypeName = t.TagType.Name
                })
                .ToListAsync(ct);
        }

        public async Task<int> GetTagsCountAsync(
            long? tagTypeId,
            string? searchTerm,
            CancellationToken ct)
        {
            var query = _context.Tags.AsNoTracking();

            if (tagTypeId.HasValue)
            {
                query = query.Where(t => t.TagTypeId == tagTypeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(t => EF.Functions.ILike(t.Name, $"%{searchTerm}%"));
            }

            return await query.CountAsync(ct);
        }
        public async Task<long> CreateAsync(Tag tag, CancellationToken ct)
        {
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync(ct);
            return tag.Id;
        }

        public async Task<bool> DeleteAsync(long tagId, CancellationToken ct)
        {
            var tag = await _context.Tags.FindAsync(new object[] { tagId }, ct);
            if (tag == null)
                return false;

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<Tag?> GetByIdAsync(long id, CancellationToken ct)
        {
            return await _context.Tags
                .Include(t => t.TagType)
                .FirstOrDefaultAsync(t => t.Id == id, ct);
        }

        public async Task<List<TagDetails>> GetRootTagsAsync(
            long? tagTypeId, string? searchTerm, long? lastId, int limit,
            CancellationToken ct)
        {
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                IQueryable<Tag> searchQuery = _context.Tags.AsNoTracking()
                    .Include(t => t.TagType)
                    .Include(t => t.ParentTag)
                    .Include(t => t.RelationType);

                if (tagTypeId.HasValue)
                    searchQuery = searchQuery.Where(t => t.TagTypeId == tagTypeId.Value);

                searchQuery = searchQuery.Where(t => EF.Functions.ILike(t.Name, $"%{searchTerm}"));

                if (lastId.HasValue)
                    searchQuery = searchQuery.Where(t => t.Id > lastId.Value);

                var searchResults = await searchQuery
                    .OrderBy(t => t.Id)
                    .Take(limit + 1)
                    .Select(t => new TagDetails
                    {
                        Id = t.Id,
                        Name = t.Name,
                        TagTypeId = t.TagTypeId,
                        TagTypeName = t.TagType.Name,
                        ParentTagId = t.ParentTagId,
                        ParentTagName = t.ParentTag != null ? t.ParentTag.Name : null,
                        RelationTypeId = t.RelationTypeId,
                        RelationTypeName = t.RelationType != null ? t.RelationType.Name : null,
                        HasChildren = _context.Tags.Any(c => c.ParentTagId == t.Id)
                    }).ToListAsync(ct);

                return searchResults;
            }

            IQueryable<Tag> query = _context.Tags.AsNoTracking()
                .Include(t => t.TagType)
                .Where(t => t.ParentTagId == null);

            if (tagTypeId.HasValue)
                query = query.Where(t => t.TagTypeId == tagTypeId.Value);

            if (lastId.HasValue)
                query = query.Where(t => t.Id > lastId.Value);

            return await query.OrderBy(t => t.Id)
                .Take(limit + 1)
                .Select(t => new TagDetails
                {
                    Id = t.Id,
                    Name = t.Name,
                    TagTypeId = t.TagTypeId,
                    TagTypeName = t.TagType.Name,
                    ParentTagId = null,
                    ParentTagName = null,
                    RelationTypeId = null,
                    HasChildren = _context.Tags.Any(c => c.ParentTagId == t.Id)
                })
                .ToListAsync(ct);
        }

        public async Task<List<TagDetails>> GetChildTagsAsync(long parentTagId,
            long ?lastId, int limit, CancellationToken ct)
        {
            IQueryable<Tag> query = _context.Tags.AsNoTracking()
                .Include(t => t.TagType)
                .Include(t => t.RelationType)
                .Where(t => t.ParentTagId == parentTagId);

            if (lastId.HasValue)
                query = query.Where(t => t.Id > lastId.Value);
            return await query.OrderBy(t => t.Id)
                .Take(limit + 1)
                .Select(t => new TagDetails
                {
                    Id = t.Id,
                    Name = t.Name,
                    TagTypeId = t.TagTypeId,
                    TagTypeName = t.TagType.Name,
                    ParentTagId = t.ParentTagId,
                    ParentTagName = null,
                    RelationTypeId = t.RelationTypeId,
                    RelationTypeName = t.RelationType != null ? t.RelationType.Name : null,
                    HasChildren = _context.Tags.Any(c => c.ParentTagId == t.Id)
                })
                .ToListAsync(ct);
        }
    }
}



