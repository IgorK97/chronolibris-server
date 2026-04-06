using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Models.Search;

namespace Chronolibris.Domain.Interfaces
{
    public interface IBookSearchRepository
    {
        Task<PagedResult<BookSearchResult>> SearchKeysetAsync(
           SimpleSearchKeysetRequest request, CancellationToken token);

        Task<PagedResult<BookSearchResult>> AdvancedSearchKeysetAsync(
            AdvancedSearchKeysetRequest request, CancellationToken token);
    }
}
