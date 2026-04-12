using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Models;

namespace Chronolibris.Domain.Interfaces
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        Task<Book?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<(List<Book> Items, int TotalCount, string? NextCursor, string? PrevCursor)> GetWithFilterAsync(
            BookFilterRequest filter, CancellationToken cancellationToken = default);
        Task<long> CreateAsync(Book book, List<PersonRoleFilter>? personFilters, CancellationToken cancellationToken = default);
        //void Update(Book book);
        void Delete(Book book);
        Task UpdateAsync(Book book, List<PersonRoleFilter>? personFilters, CancellationToken ct);
        Task<List<string>> GetAuthorNamesByBookIdAsync(long bookId, CancellationToken cancellationToken = default);
        Task<List<Theme>> GetThemesByBookIdAsync(long bookId, CancellationToken cancellationToken = default);

        Task<int> GetContentsCountAsync(long bookId, CancellationToken cancellationToken = default);
        Task<List<Content>> GetContentsByBookIdAsync(long bookId, CancellationToken cancellationToken = default);
        Task LinkContentToBookAsync(long bookId, long contentId, CancellationToken cancellationToken = default);
        Task UnlinkContentFromBookAsync(long bookId, long contentId, CancellationToken cancellationToken = default);
        Task<bool> IsLinkedToContentAsync(long bookId, long contentId, CancellationToken cancellationToken = default);
        Task<BookDetails?> GetBookWithRelationsAsync(long bookId, long userId, bool mode, CancellationToken token = default);
        Task<List<BookListItem>>
            GetSearchedBooks(string query, long? lastId, int limit, long userId, CancellationToken token = default);
    }
}
