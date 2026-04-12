using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using Chronolibris.Domain.Utils;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.Persistance.Repositories
{

    public class SelectionsRepository : ISelectionsRepository
    {
        private readonly ApplicationDbContext _context;


        public SelectionsRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<Selection?> GetByIdAsync(long id, long userId, string userRole, CancellationToken ct)
        {
            var selection = await _context.Selections
                .Include(s => s.Books)
                .FirstOrDefaultAsync(s => s.Id == id, ct);
            if ((userId == 0 || !(userRole == "admin" || userRole == "moderator")) && selection?.IsActive == false)
                return null;
            return selection;
        }

        public async Task<IEnumerable<Selection>> GetActiveSelectionsAsync(CancellationToken ct)
        {
            return await _context.Selections
                .Where(s => s.IsActive)
                .ToListAsync(ct);
        }

        public async Task<List<SelectionDetails>> GetSelectionsAsync(
            long? lastId,
            int limit,
            bool? onlyActive,
            CancellationToken ct)
        {

            if (limit < 1) limit = 20;
            else if (limit > 100) limit = 100;

            var query = _context.Selections.AsNoTracking();

            // Фильтр по активности
            if (onlyActive.HasValue)
                query = query.Where(s => s.IsActive == onlyActive.Value);

            // Keyset cursor
            if (lastId.HasValue)
                query = query.Where(s => s.Id > lastId.Value);

            // Берём limit + 1, чтобы понять — есть ли следующая страница
            var items = await query
                .OrderBy(s => s.Id)
                .Take(limit)
                .Select(s => new SelectionDetails
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    BooksCount = s.Books.Count()
                })
                .ToListAsync(ct);
            return items;

        }


        public async Task<List<BookListItem>>
            GetBooksForSelection(long selectionId, long? lastId, int limit, long userId, bool mode, CancellationToken ct)
        {


            var query = _context.Books.AsNoTracking()
                .Where(b => b.Selections.Any(s => s.Id == selectionId));

            if (!mode)
                query = query.Where(b => b.IsAvailable);

            if (lastId.HasValue)
            {
                query = query.Where(b => b.Id > lastId.Value);
            }



            var books = await query
                .OrderBy(rp => rp.Id)
                .Select(b => new BookListItem
                {
                    Id = b.Id,
                    Title = b.Title,
                    //AverageRating = b.AverageRating,
                    IsReviewable=b.IsReviewable,
                    CoverUri = b.CoverPath,
                    AverageRating = b.Reviews.Any() ? b.Reviews.Average(r => (decimal)r.Score) : 0.0M,
                    RatingsCount = b.IsReviewable ? b.Reviews.Count() : 0,
                    //RatingsCount = b.RatingsCount,
                    IsFavorite = b.Shelves.Any(s =>
                        s.UserId == userId &&
                        s.ShelfType.Code == ShelfTypes.FAVORITES),

                    IsRead = b.Shelves.Any(s =>
                        s.UserId == userId &&
                        s.ShelfType.Code == ShelfTypes.READ),

                    Authors = b.BookContents
                        .SelectMany(bc => bc.Content.Participations
                        .Where(p => p.PersonRoleId == (Int64) PersonRoles.Author)
                            .Select(p => p.Person.Name))
                        .Distinct()
                        .ToList()
                })
                .Take(limit + 1)
                .ToListAsync(ct);

            return books;

        }

        public async Task<long> CreateAsync(Selection selection, CancellationToken ct)
        {
            _context.Selections.Add(selection);
            await _context.SaveChangesAsync(ct);
            return selection.Id;
        }

        public async Task<bool> UpdateAsync(long selectionId, string? name, string? description, bool? isActive, CancellationToken ct)
        {
            var selection = await _context.Selections.FindAsync(new object[] { selectionId }, ct);
            if (selection == null) return false;

            if (name != null) selection.Name = name;
            if (description != null) selection.Description = description;
            if (isActive.HasValue) selection.IsActive = isActive.Value;
            selection.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return true;
        }
        public async Task<bool> AddBookToSelectionAsync(long selectionId, long bookId, CancellationToken ct)
        {
            var selection = await _context.Selections
                .Include(s => s.Books)
                .FirstOrDefaultAsync(s => s.Id == selectionId, ct);

            if (selection == null) return false;

            var book = await _context.Books.FindAsync(new object[] { bookId }, ct);
            if (book == null) return false;

            if (!selection.Books.Any(b => b.Id == bookId))
            {
                selection.Books.Add(book);
                await _context.SaveChangesAsync(ct);
            }

            return true;
        }
        public async Task<bool> RemoveBookFromSelectionAsync(long selectionId, long bookId, CancellationToken ct)
        {
            var selection = await _context.Selections
                .Include(s => s.Books)
                .FirstOrDefaultAsync(s => s.Id == selectionId, ct);

            if (selection == null) return false;

            var book = selection.Books.FirstOrDefault(b => b.Id == bookId);
            if (book != null)
            {
                selection.Books.Remove(book);
                await _context.SaveChangesAsync(ct);
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteAsync(long selectionId, CancellationToken ct)
        {
            var selection = await _context.Selections.FindAsync(new object[] { selectionId }, ct);
            if (selection == null) return false;

            _context.Selections.Remove(selection);
            await _context.SaveChangesAsync(ct);
            return true;
        }
    }

}
