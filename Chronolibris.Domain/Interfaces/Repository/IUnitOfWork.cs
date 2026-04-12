using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
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
        IGenericRepository<Person> Persons { get; }
        IGenericRepository<Country> Countries { get; }
        IGenericRepository<Format> Formats { get; }
        IGenericRepository<Language> Languages { get; },
        IGenericRepository<Content> Contents { get; }
        IGenericRepository<Publisher> Publishers { get; }
        IGenericRepository<PersonRole> PersonRoles { get; }
        IReadingProgressRepository ReadingProgresses { get; }
        Task<int> SaveChangesAsync(CancellationToken token = default);
        Task<ITransaction> BeginTransactionAsync(CancellationToken token = default);
    }

    public interface ITransaction : IAsyncDisposable
    {
        Task CommitAsync(CancellationToken token = default);
        Task RollbackAsync(CancellationToken token = default);
    }
}
