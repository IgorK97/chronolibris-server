using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Entities
{
    public class BookShelf
    {
        public long BookId { get; set; }
        public Book Book { get; set; } = null!;

        public long ShelfId { get; set; }
        public Shelf Shelf { get; set; } = null!;
        public DateTime AddedAt { get; set; }
    }
}
