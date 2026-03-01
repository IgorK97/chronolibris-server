using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Entities
{
    public class Person
    {
        public required long Id { get; set; }
        [MaxLength(255)]
        public required string Name { get; set; }
        public required string Description { get; set; }
        [MaxLength(2048)]
        public required string ImagePath { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<Content> Contents { get; set; } = new List<Content>();
        public ICollection<Book> Books { get; set; } = new List<Book>();
        public ICollection<BookParticipation> BookParticipations { get; set; } = new List<BookParticipation>();
        public ICollection<ContentParticipation> ContentParticipations { get; set; } = new List<ContentParticipation>();
    }
}
