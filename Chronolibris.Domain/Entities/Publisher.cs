using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Entities
{
    public class Publisher
    {
        public required long Id { get; set; }
        [MaxLength(256)]
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public required long CountryId { get; set; }
        //public ICollection<Series> Series { get; set; } = new List<Series>();
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
