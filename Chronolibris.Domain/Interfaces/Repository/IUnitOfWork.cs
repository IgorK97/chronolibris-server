using Chronolibris.Domain.Entities;

namespace Chronolibris.Domain.Interfaces.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IBookRepository Books { get; }
        IBookmarkRepository Bookmarks { get; }
        IReportRepository Reports { get; }
        IModerationTasksRepository ModerationTasks { get; }
        IReviewReactionsRepository ReviewReactions { get; }
        ICommentReactionsRepository CommentReactions { get; }
        IBookFileRepository BookFiles { get; }
        IReviewRepository Reviews { get; }
        ISelectionsRepository Selections { get; }
        IShelfRepository Shelves { get; }
        ICommentRepository Comments { get; }
        IThemeRepository Themes { get; }
        IGenericRepository<Person> Persons { get; }
        IGenericRepository<Country> Countries { get; }
        IGenericRepository<Format> Formats { get; }
        IGenericRepository<Language> Languages { get; }
        IContentRepository Contents { get; }
        IGenericRepository<Publisher> Publishers { get; }
        IGenericRepository<PersonRole> PersonRoles { get; }
        Task<int> SaveChangesAsync(CancellationToken token = default);
        Task<ITransaction> BeginTransactionAsync(CancellationToken token = default);
    }

    public interface ITransaction : IAsyncDisposable
    {
        Task CommitAsync(CancellationToken token = default);
        Task RollbackAsync(CancellationToken token = default);
    }
}
