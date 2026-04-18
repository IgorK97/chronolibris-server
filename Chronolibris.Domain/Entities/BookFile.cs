        using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

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
        public int FormatId { get; set; }
        //public required int MediaTypeId { get; set; }
        [MaxLength(2048)]
        [Required]
        public string StorageUrl { get; set; } = String.Empty;
        [Required]
        public long FileSizeBytes { get; set; }
        [Required]
        public bool IsReadable { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        //public required DateTime UpdatedAt { get; set; }
        [Required]
        public long CreatedBy { get; set; }
        //public required int Version { get; set; } = 0;
        [Required]
        public long BookFileStatusId { get; set; }
        [Required]
        public long MaxParaIndex { get; set; }

        public Book Book { get; set; } = null!;
        public Format Format { get; set; } = null!;
        //public MediaType MediaType { get; set; } = null!;
        public ICollection<BookFragment> Fragments { get; set; } = [];
        public BookFileStatus BookFileStatus { get; set; }
        public ICollection<Bookmark> Bookmarks { get; set; } = [];
        public ICollection<ReadingProgress> Readings { get; set; } = [];

    }
}
