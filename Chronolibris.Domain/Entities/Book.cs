using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Entities
{
    public class Book
    {
        [Key]
        public long Id { get; set; }
        [MaxLength(500)]
        [Required]
        public string Title { get; set; } = String.Empty;
        [MaxLength(2000)]
        public string Description { get; set; } = String.Empty;
        [Required]
        public long CountryId { get; set; }
        [Required]
        public long LanguageId { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [ConcurrencyCheck]
        public DateTime? UpdatedAt { get; set; }
        public int? Year { get; set; }
        [MaxLength(150)]
        public string? Bbk { get; set; }
        [MaxLength(150)]
        public string? Udk { get; set; }
        [MaxLength(17)]
        public string? ISBN { get; set; }
        //public required bool IsFragment { get; set; }
        //[MaxLength(2048)]
        //public required string FilePath { get; set; }
        [MaxLength(2048)]
        [Required]
        public string CoverPath { get; set; } = String.Empty;

        //public string? SearchData { get; set; }
        [Required]
        public bool IsAvailable { get; set; }
        [Required]
        public bool IsReviewable { get; set; }
        public long? PublisherId { get; set; }
        [MaxLength(500)]
        public string? Source { get; set; }
        public Publisher? Publisher { get; set; }
        //public long? SeriesId { get; set; }
        //public Series? Series { get; set; }
        public Country Country { get; set; } = null!;
        public Language Language { get; set; } = null!;
        public ICollection<Review> Reviews { get; set; } = [];
        public ICollection<BookContent> BookContents { get; set; } = [];
        public ICollection<Shelf> Shelves { get; set; } = [];
        public ICollection<BookShelf> BookShelves { get; set; } = [];
        public ICollection<Person> Persons { get; set; } = [];
        public ICollection<BookParticipation> Participations { get; set; } = [];
        public ICollection<Selection> Selections { get; set; } = [];
        public ICollection<Comment> Comments { get; set; } = [];
        
    }
}
