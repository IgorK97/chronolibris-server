using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Models.Search
{
    public class OffsetPagedResult<T>
    {
        public List<T> Items { get; set; } = [];
        public bool HasNext { get; set; }
        public int Page { get; set; }
        public int TotalCount { get; set; }
    }
}
