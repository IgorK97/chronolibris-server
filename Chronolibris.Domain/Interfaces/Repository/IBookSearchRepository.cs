using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Models.Search;

namespace Chronolibris.Domain.Interfaces.Repository
{
    public interface IBookSearchRepository
    {
        Task<PagedBooks<BookSearchResult>> SearchKeysetAsync(
           SimpleSearchKeysetRequest request, CancellationToken token);

        Task<PagedBooks<BookSearchResult>> AdvancedSearchKeysetAsync(
            AdvancedSearchKeysetRequest request, CancellationToken token);
    }
}
