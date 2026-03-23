using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Entities
{
    public class Shelf
    {
        public required long Id { get; set; }
        public required long UserId { get; set; }
        public required long ShelfTypeId { get; set; }
        [MaxLength(256)]
        public required string Name { get; set; }
        public required DateTime CreatedAt { get; set; }
        public ShelfType ShelfType { get; set; } = null!;

        public ICollection<Book> Books { get; set; } = new List<Book>();
        public ICollection<BookShelf> BookShelves { get; set; } = new List<BookShelf>();
    }
}
