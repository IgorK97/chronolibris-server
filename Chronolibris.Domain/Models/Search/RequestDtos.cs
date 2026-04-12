using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Models.Search
{
    public class SimpleSearchKeysetRequest
    {
        public required string Query { get; set; }
        public int PageSize { get; set; } = 20;
        public long? UserId { get; set; }
        public double? LastBestSimilarity { get; set; }
        public long? LastId { get; set; }
        public bool mode { get; set; }
    }

    public class AdvancedSearchKeysetRequest
    {
        public string? Query { get; set; }
        public int PageSize { get; set; } = 20;

        public List<PersonRoleFilter> PersonFilters { get; set; } = [];
        public long ThemeId { get; set; }
        public required long SelectionId { get; set; }
        public List<long> RequiredTagIds { get; set; } = [];
        public List<long> ExcludedTagIds { get; set; } = [];
        public long? UserId { get; set; }
        public double? LastBestSimilarity { get; set; }
        public long? LastId { get; set; }
        public bool mode { get; set; }
    }

}
