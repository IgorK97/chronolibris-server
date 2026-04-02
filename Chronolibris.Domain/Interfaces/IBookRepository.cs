using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Models;

namespace Chronolibris.Domain.Interfaces
{
    /// <summary>
    /// Определяет контракт для репозитория, управляющего сущностями <see cref="Book"/>.
    /// Наследует базовые операции CRUD от <see cref="IGenericRepository{T}"/>.
    /// </summary>
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
        Task LinkContentToBookAsync(long bookId, long contentId, int order, CancellationToken cancellationToken = default);
        Task UnlinkContentFromBookAsync(long bookId, long contentId, CancellationToken cancellationToken = default);
        Task<bool> IsLinkedToContentAsync(long bookId, long contentId, CancellationToken cancellationToken = default);
        /// <summary>
        /// Асинхронно получает сущность книги по ее идентификатору, включая все связанные 
        /// с ней данные (например, авторов, издателя, рецензии и т.д.).
        /// Используется для получения полного профиля книги.
        /// </summary>
        /// <param name="id">Уникальный идентификатор книги.</param>
        /// <param name="token">Токен отмены для прерывания операции. По умолчанию — <c>default</c>.</param>
        /// <returns>
        /// Задача, которая представляет асинхронную операцию. Результат задачи — 
        /// сущность <see cref="Book"/> или <c>null</c>, если книга не найдена.
        /// </returns>
        Task<BookDetails?> GetBookWithRelationsAsync(long bookId, long userId, CancellationToken token = default);
        Task<List<BookListItem>>
            GetSearchedBooks(string query, long? lastId, int limit, long userId, CancellationToken token = default);
    }
}
