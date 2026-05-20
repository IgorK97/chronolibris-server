using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Entities
{
    public class BookSelection
    {
        public long BookId { get; set; }
        public long SelectionId { get; set; }
        public long AddedBy { get; set; }
        public DateTime AddedAt { get; set; }
        public long? HiddenBy { get; set; }
        public DateTime? HiddenAt { get; set; }
    }
}
