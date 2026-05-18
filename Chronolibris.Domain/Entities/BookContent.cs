using System.ComponentModel.DataAnnotations.Schema;

namespace Chronolibris.Domain.Entities
{
    [Table("book_content")]
    public class BookContent
    {
        public long ContentId { get; set; }
        public long BookId { get; set; }
        public Content Content { get; set; }
        public Book Book { get; set; }
    }
}
