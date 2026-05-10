using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Domain.Entities
{
    public class Publisher
    {
        public required long Id { get; set; }
        [MaxLength(255)]
        public required string Name { get; set; }
        [MaxLength(5000)]
        public required string Description { get; set; }
        public required DateTime CreatedAt { get; set; }
        //public required long CountryId { get; set; }
        //public ICollection<Series> Series { get; set; } = new List<Series>();
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
