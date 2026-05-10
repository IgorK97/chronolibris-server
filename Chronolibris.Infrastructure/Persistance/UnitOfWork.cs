using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Chronolibris.Infrastructure.Persistance
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IBookRepository Books { get; }
        public IBookmarkRepository Bookmarks { get; }
        public IReportRepository Reports { get; }
        public IReviewReactionsRepository ReviewReactions { get; }
        public ICommentReactionsRepository CommentReactions { get; }
        public IBookFileRepository BookFiles { get; }
        public IReviewRepository Reviews { get; }
        public ICommentRepository Comments { get; }
        public IThemeRepository Themes { get; }
        public ISelectionsRepository Selections { get; }
        public IShelfRepository Shelves { get; }
        public IContentRepository Contents { get; }
        public IGenericRepository<Person> Persons { get; }
        public IGenericRepository<Language> Languages { get; }
        public IGenericRepository<Country> Countries { get; }
        public IGenericRepository<Format> Formats { get; }
        public IGenericRepository<Publisher> Publishers { get; }
        public IGenericRepository<PersonRole> PersonRoles { get; }
        public IModerationTasksRepository ModerationTasks { get; }

        public UnitOfWork(ApplicationDbContext context, IBookRepository bookRepository,
            IBookmarkRepository bookmarks,
            IGenericRepository<Person> personRepository, IContentRepository contentRepository,
            IGenericRepository<Publisher> publisherRepository,
            IReviewReactionsRepository reviewsRatings,
            IReviewRepository reviewRepository,
            ISelectionsRepository selections, IShelfRepository shelves, ICommentRepository comments,
            IGenericRepository<PersonRole> personRoles,
            ICommentReactionsRepository commentReactions, IGenericRepository<Language> languages,
            IGenericRepository<Country> countries, IGenericRepository<Format> formats,
            IModerationTasksRepository moderationTasks,
            IBookFileRepository bookFiles, IReportRepository reports, IThemeRepository themes)
        {
            _context = context;

            Books = bookRepository;
            Bookmarks = bookmarks;
            Persons = personRepository;
            Contents = contentRepository;
            Reviews = reviewRepository;
            Publishers = publisherRepository;
            ReviewReactions = reviewsRatings;
            Selections = selections;
            Shelves = shelves;
            PersonRoles = personRoles;
            Comments = comments;
            CommentReactions = commentReactions;
            Languages = languages;
            Countries = countries;
            Formats = formats;
            //Series = series;
            BookFiles = bookFiles;
            Reports = reports;
            ModerationTasks = moderationTasks;
            Themes = themes;
        }
        public async Task<int> SaveChangesAsync(CancellationToken ct)
        {
            try
            {
                return await _context.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ChronolibrisException("Ошибка обновления данных - повторите попытку позднее", ErrorType.Conflict);
            }
        }

        public async Task<ITransaction> BeginTransactionAsync(
            CancellationToken token = default)
        {
            var tx = await _context.Database.BeginTransactionAsync(token);
            return new EfTransaction(tx);
        }
        public void Dispose() => _context.Dispose();
    }

    public sealed class EfTransaction : ITransaction
    {
        private readonly IDbContextTransaction _inner;
        public EfTransaction(IDbContextTransaction inner)
        {
            _inner = inner;

        }

        public Task CommitAsync(CancellationToken token = default)
        {
            return _inner.CommitAsync(token);
        }
        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            return _inner.RollbackAsync(cancellationToken);
        }

        public ValueTask DisposeAsync() => _inner.DisposeAsync();
    }
}
