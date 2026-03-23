using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Entities
{
    public class Language
    {
        public required long Id { get; set; }
        [MaxLength(50)]
        public required string Name { get; set; }
        [MaxLength(50)]
        public string? Code { get; set; }
        public ICollection<Book> Books { get; set; } = new List<Book>();
        public ICollection<Content> Contents { get; set; } = new List<Content>();
    }
}
