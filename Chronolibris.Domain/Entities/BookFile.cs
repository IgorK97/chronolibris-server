using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Domain.Entities
{
    public class BookFile
    {
        [Key]
        [Required]
        public long Id { get; set; }
        [Required]
        public long BookId { get; set; }
        [Required]
        public long FormatId { get; set; }
        [MaxLength(2048)]
        [Required]
        public string StorageUrl { get; set; } = String.Empty;
        [Required]
        public long OriginalSize { get; set; }
        public long StoredSize { get; set; }
        [Required]
        public bool IsReadable { get; set; }
        public bool? HistoricalText { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public long CreatedBy { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? HiddenAt { get; set; }
        //public long? HiddenBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        //public long? DeletedBy { get; set; }
        //public required DateTime UpdatedAt { get; set; }
        //[Required]
        //public long CreatedBy { get; set; }
        //public required int Version { get; set; } = 0;
        [Required]
        [ConcurrencyCheck]
        public long StatusId { get; set; }
        [Required]
        public Book Book { get; set; } = null!;
        public Format Format { get; set; } = null!;
        public ICollection<BookFragment> Fragments { get; set; } = [];
        public BookFileStatus BookFileStatus { get; set; }
        public ICollection<Bookmark> Bookmarks { get; set; } = [];
        public ICollection<ReadingProgress> Readings { get; set; } = [];

    }
}
