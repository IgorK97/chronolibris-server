using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Entities
{
    [Table("book_content")]
    public class BookContent
    {
        public long ContentId { get; set; }
        public long BookId { get; set; }
        public Content Content { get; set; }
        public Book Book { get; set; }
        //public int Order { get; set; }
    }
}
