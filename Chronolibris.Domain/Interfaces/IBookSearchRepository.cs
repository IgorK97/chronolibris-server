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
        //Простой поиск по названию,
        //офсетная пагинация
        Task<OffsetPagedResult<BookSearchResult>> SearchOffsetAsync(
            SimpleSearchOffsetRequest request, CancellationToken token);

        //Расширенный поиск, офсетная пагинация
        Task<OffsetPagedResult<BookSearchResult>> AdvancedSearchOffsetAsync(
            AdvancedSearchOffsetRequest request, CancellationToken token);
    }
}
