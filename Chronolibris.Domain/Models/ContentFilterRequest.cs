using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Models
{
    public class ContentFilterRequest
    {
        public string? SearchQuery { get; set; }
        public string? AuthorName { get; set; }
        public List<PersonRoleFilter>? PersonFilters { get; set; }
        public string? Cursor { get; set; }
        public int Limit { get; set; } = 20;
    }
}
