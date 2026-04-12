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
using Chronolibris.Infrastructure.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.DataAccess.Persistance.Repositories
{
    public class ReadingProgressRepository : GenericRepository<ReadingProgress>, IReadingProgressRepository
    {
        public ReadingProgressRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<ReadingProgress?> GetForBookUser(long bookId, long userId, CancellationToken token)
        {
            return await _context.ReadingProgresses.Where(rp => rp.UserId == userId && rp.BookFileId == bookId).FirstOrDefaultAsync(token);
        }


        public async Task<List<BookListItem>> GetBooks(long userId, long? lastId, int limit, CancellationToken ct)
        {

            var query = _context.ReadingProgresses.AsNoTracking()
                .Where(rp => rp.UserId == userId);

            // Потом попробовать переписать логику
            if (lastId.HasValue)
            {
                query = query.Where(rp => rp.Id > lastId.Value);
            }

            var books = await query
                .OrderBy(rp => rp.Id)
                .Select(rp => new BookListItem
                {
                    Id = rp.Id,
                    Title = rp.BookFile.Book.Title,
                    //AverageRating = rp.Book.AverageRating,
                    CoverUri = rp.BookFile.Book.CoverPath,
                    IsReviewable=rp.BookFile.Book.IsReviewable,
                    //RatingsCount = rp.Book.RatingsCount,
                    AverageRating = rp.BookFile.Book.Reviews.Any() ? rp.BookFile.Book.Reviews.Average(r => (decimal)r.Score) : 0.0M,
                    RatingsCount = rp.BookFile.Book.IsReviewable ? rp.BookFile.Book.Reviews.Count() : 0,
                    IsFavorite = rp.BookFile.Book.Shelves.Any(s =>
                        s.UserId == userId &&
                        s.ShelfType.Code == ShelfTypes.FAVORITES),


                    IsRead = rp.BookFile.Book.Shelves.Any(s =>
                        s.UserId == userId &&
                        s.ShelfType.Code == ShelfTypes.READ),

                    Authors = rp.BookFile.Book.BookContents
                        .SelectMany(bc => bc.Content.Participations
                            .Select(p => p.Person.Name))
                        .ToList()
                })
                .Take(limit + 1)
                .ToListAsync(ct);

            return books;
        }


    }
}
