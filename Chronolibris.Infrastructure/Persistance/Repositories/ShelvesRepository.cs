using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using Chronolibris.Domain.Utils;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.Persistance.Repositories
{

    public class ShelvesRepository : GenericRepository<Shelf>, IShelfRepository
    {

        public ShelvesRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Shelf?> GetByIdAsync(long shelfId, CancellationToken ct)
        {
            return await _context.Shelves
                .Include(s => s.Books)
                .FirstOrDefaultAsync(s => s.Id == shelfId, ct);
        }


        public async Task<IEnumerable<Shelf>> GetForUserAsync(long userId, CancellationToken ct)
        {
            return await _context.Shelves
                .Where(s => s.UserId == userId)
                .Include(s => s.Books)
                .ToListAsync(ct);
        }


        public async Task<List<BookListItem>>
            GetBooksForShelfAsync(long shelfId, long? lastId, int limit, long userId, CancellationToken ct)
        {
            var query = _context.Books
                .AsNoTracking()
                .Where(b => b.Shelves.Any(s => s.Id == shelfId));

            if (lastId.HasValue)
                query = query.Where(b => b.Id > lastId.Value);

            var books = await query
                .OrderBy(b => b.Id)
                .Select(b => new BookListItem
                {
                    Id = b.Id,
                    Title = b.Title,
                    //AverageRating = b.AverageRating,
                    CoverUri = b.CoverPath,
                    IsReviewable = b.IsReviewable,  
                    //RatingsCount = b.RatingsCount,
                    AverageRating = b.Reviews.Any() ? b.Reviews.Average(r => (decimal)r.Score) : 0.0M,
                    RatingsCount = b.IsReviewable ? b.Reviews.Count() : 0,
                    IsFavorite = b.Shelves.Any(s =>
                        s.UserId == userId &&
                        s.ShelfType.Code == ShelfTypes.FAVORITES),

                    IsRead = b.Shelves.Any(s =>
                        s.UserId == userId &&
                        s.ShelfType.Code == ShelfTypes.READ),

                    Authors = b.BookContents
                        .SelectMany(bc => bc.Content.Participations
                            .Select(p => p.Person.Name))
                        .ToList()
                })
                .Take(limit + 1)
                .ToListAsync(ct);

            return books;
        }

        public async Task AddBookToShelf(long shelfId, long bookId, CancellationToken ct)
        {
            var shelf = await _context.Shelves
                .Include(s => s.Books)
                .FirstAsync(s => s.Id == shelfId, ct);

            if (!shelf.Books.Any(b => b.Id == bookId))
            {
                var book = await _context.Books.FindAsync(bookId);
                if (book != null)
                    shelf.Books.Add(book);
            }
        }

        public async Task RemoveBookFromShelf(long shelfId, long bookId, CancellationToken ct)
        {
            var shelf = await _context.Shelves
                .Include(s => s.Books)
                .FirstAsync(s => s.Id == shelfId, ct);

            var book = shelf.Books.FirstOrDefault(b => b.Id == bookId);
            if (book != null)
                shelf.Books.Remove(book);
        }

        public async Task<bool> IsInFavorite(long userId, long bookId)
        {
            return await _context.Shelves
                .AnyAsync(s =>
                    s.UserId == userId &&
                    s.ShelfType.Code == ShelfTypes.FAVORITES &&
                    s.Books.Any(b => b.Id == bookId));
        }

        public async Task<bool> IsRead(long userId, long bookId)
        {
            return await _context.Shelves
                .AnyAsync(s =>
                    s.UserId == userId &&
                    s.ShelfType.Code == ShelfTypes.READ &&
                    s.Books.Any(b => b.Id == bookId));
        }

        public async Task<bool> IsInShelf(long userId, long shelfId)
        {
            return await _context.Shelves
                .AnyAsync(s => s.UserId == userId && s.Id == shelfId);
        }

        public async Task<long[]> SeekBookInShelves(long userId, long bookId)
        {
            return await _context.Shelves
                .Where(s => s.UserId == userId && s.Books.Any(b => b.Id == bookId))
                .Select(s => s.Id)
                .ToArrayAsync();
        }
    }

}
