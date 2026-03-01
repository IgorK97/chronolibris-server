        using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Entities
{
    public class DigitalFile
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
        public required DateTime UpdatedAt { get; set; }

        public Book Book { get; set; } = null!;
        public Format Format { get; set; } = null!;
        //public MediaType MediaType { get; set; } = null!;

    }
}
