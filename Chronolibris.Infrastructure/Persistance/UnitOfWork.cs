using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Handlers;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Infrastructure.Data;
using Chronolibris.Infrastructure.Persistance.Repositories;
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
        public ISelectionsRepository Selections { get; }
        public IShelfRepository Shelves { get; }
        public IGenericRepository<Person> Persons { get; }
        public ILanguageRepository Languages { get; }
        public IGenericRepository<Content> Contents { get; }
        public IGenericRepository<Country> Countries { get; }
        public IGenericRepository<Format> Formats { get; }
        public IGenericRepository<Publisher> Publishers { get; }
        public IGenericRepository<PersonRole> PersonRoles { get; }
        public IReadingProgressRepository ReadingProgresses { get; }
        public IModerationTasksRepository ModerationTasks { get; }

        public UnitOfWork(ApplicationDbContext context, IBookRepository bookRepository,
            IBookmarkRepository bookmarks,
            IGenericRepository<Person> personRepository, IGenericRepository<Content> contentRepository,
            IGenericRepository<Publisher> publisherRepository,
            IReviewReactionsRepository reviewsRatings,
            IReviewRepository reviewRepository,
            ISelectionsRepository selections, IShelfRepository shelves, ICommentRepository comments,
            IGenericRepository<PersonRole> personRoles, IReadingProgressRepository readingProgresses, 
            ICommentReactionsRepository commentReactions, ILanguageRepository languages,
            IGenericRepository<Country> countries, IGenericRepository<Format> formats,
            IModerationTasksRepository moderationTasks,
            IBookFileRepository bookFiles, IReportRepository reports)
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
            ReadingProgresses = readingProgresses;
            Comments = comments;
            CommentReactions = commentReactions;
            Languages = languages;
            Countries = countries;
            Formats = formats;
            //Series = series;
            BookFiles = bookFiles;
            Reports = reports;
            ModerationTasks = moderationTasks;
        }
        public async Task<int> SaveChangesAsync(CancellationToken ct) =>
        await _context.SaveChangesAsync(ct);

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
