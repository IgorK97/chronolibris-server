using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Models;

namespace Chronolibris.Domain.Interfaces.Repository
{
    public interface ISelectionsRepository : IGenericRepository<Selection>
    {
        Task<Selection?> GetByIdAsync(long id, long userId, string userRole, CancellationToken token = default);
        Task<IEnumerable<Selection>> GetActiveSelectionsAsync(CancellationToken token = default);
        Task<List<SelectionDetails>> GetSelectionsAsync(
            long? lastId,
            int limit,
            bool? onlyActive, CancellationToken token = default);
        Task<List<BookListItem>>
            GetBooksForSelection(long selectionId, long? lastId, int limit, long userId, bool mode, CancellationToken token = default);
        Task<bool> DeleteAsync(long id, CancellationToken ct);
        Task<bool> IsBookInSelection(long bookId, long selectionId, CancellationToken token = default);
        Task<long> CreateAsync(Selection selection, CancellationToken ct);
        Task<bool> UpdateAsync(long selectionId, string? name, string? description, bool? isActive, CancellationToken ct);
        Task<bool> AddBookToSelectionAsync(long selectionId, long bookId, CancellationToken ct);
        Task<bool> RemoveBookFromSelectionAsync(long selectionId, long bookId, CancellationToken ct);
        //Task<bool> DeleteAsync(long selectionId, CancellationToken ct);
    }

}
