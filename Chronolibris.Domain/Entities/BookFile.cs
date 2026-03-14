        using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Entities
{
    public static class BookFileStatuses{
        public static readonly int PENDING = 1;
        public static readonly int UPLOADED = 2;
        public static readonly int PROCESSING = 3;
        public static readonly int COMPLETED = 4;
        public static readonly int FAILED = 5;
    }
    public class BookFile
    {
        [Key]
        public required long Id { get; set; }
        public required long BookId { get; set; }
        public required int FormatId { get; set; }
        //public required int MediaTypeId { get; set; }
        [MaxLength(2048)]
        public required string StorageUrl { get; set; }
        public required long FileSizeBytes { get; set; }

        public required bool IsReadable { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        //public required DateTime UpdatedAt { get; set; }
        public required long CreatedBy { get; set; }
        public required int Version { get; set; } = 0;
        public required long BookFileStatusId { get; set; }

        public Book Book { get; set; } = null!;
        public Format Format { get; set; } = null!;
        //public MediaType MediaType { get; set; } = null!;
        public ICollection<BookFragment> Fragments { get; set; }
        public BookFileStatus BookFileStatus { get; set; }

    }
}
