using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Models.Search
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = [];
        public bool HasNext { get; set; }
        public long? LastId { get; set; }
        public double? LastBestSimilarity { get; set; }
    }
}
