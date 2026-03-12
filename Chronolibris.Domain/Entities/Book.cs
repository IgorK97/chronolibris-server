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
        public required long Id { get; set; }
        [MaxLength(500)]
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required long CountryId { get; set; }
        public required long LanguageId { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? Year { get; set; }
        [MaxLength(17)]
        public string? ISBN { get; set; }
        public required bool IsFragment { get; set; }
        [MaxLength(2048)]
        public required string FilePath { get; set; }
        [MaxLength(2048)]
        public required string CoverPath { get; set; }

        //public string? SearchData { get; set; }
        public required bool IsAvailable { get; set; }

        public required bool IsReviewable { get; set; }
        public long? PublisherId { get; set; }
        public Publisher? Publisher { get; set; }
        public long? SeriesId { get; set; }
        public Series? Series { get; set; }
        public Country Country { get; set; } = null!;
        public Language Language { get; set; } = null!;
        public ICollection<Bookmark> Bookmarks { get; set; } = [];
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
