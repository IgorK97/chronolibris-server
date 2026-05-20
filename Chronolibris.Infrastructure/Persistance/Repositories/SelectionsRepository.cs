using Chronolibris.Application.Models;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using Chronolibris.Domain.Utils;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Chronolibris.Infrastructure.Persistance.Repositories
{

    public class SelectionsRepository : GenericRepository<Selection>, ISelectionsRepository
    {

        public SelectionsRepository(ApplicationDbContext context) : base(context) { }

        public async Task<bool> IsBookInSelection(long bookId, long selectionId, CancellationToken token = default)
        {
            return await _context.Selections
                        .Where(s => s.Id == selectionId)
                        .AnyAsync(s => s.Books.Any(b => b.Id == bookId), token);
        }
        public async Task<bool> DeleteAsync(long id, CancellationToken ct)
        {
            var rowsAffected = await _context.Selections
                .Where(s => s.Id == id)
                .ExecuteDeleteAsync(ct);

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Selection>> GetActiveSelectionsAsync(CancellationToken ct)
        {
            return await _context.Selections
                .Include(s => s.Books) //потом проекцию сделать, но книг так-то немного в подборках должно быть,
                                       //да и самих подборок сейчас немного (и активных всегда немного будет),
                                       //поэтому можно пока так оставить
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

            if (onlyActive.HasValue)
                query = query.Where(s => s.IsActive == onlyActive.Value);

            if (lastId.HasValue)
                query = query.Where(s => s.Id > lastId.Value);

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
                        .Where(p => p.PersonRoleId == 1)
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
            await _context.SaveChangesAsync(ct); //Пока здесь оставлю
            return selection.Id;
        }
        public async Task AddBookToSelectionAsync(long selectionId, long bookId, CancellationToken ct)
        {
            var selection = await _context.Selections
                .Include(s => s.Books)
                .FirstOrDefaultAsync(s => s.Id == selectionId, ct);

            if (selection == null)
                throw new ChronolibrisException("Книга или подборка не найдена", ErrorType.NotFound);

            var book = await _context.Books.FindAsync(new object[] { bookId }, ct);
            if (book == null) 
                throw new ChronolibrisException("Книга или подборка не найдена", ErrorType.NotFound);

            try
            {
                if (!selection.Books.Any(b => b.Id == bookId))
                {
                    selection.Books.Add(book);
                    await _context.SaveChangesAsync(ct);
                }
            } catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
            {
                if (pgEx.SqlState == "23503")
                {
                    throw new ChronolibrisException("Книга или подборка не найдена", ErrorType.NotFound);
                }
                if (pgEx.SqlState == "23505")
                    return;
                throw;
            }
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

        public Task<List<long>> SeekBookInSelections(long bookId, CancellationToken ct)
        {
            return _context.Selections.Where(s => s.Books.Any(b => b.Id == bookId))
                .Select(s => s.Id).ToListAsync(ct);
        }
    }
}