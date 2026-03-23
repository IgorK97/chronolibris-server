using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Entities
{
    public class BookFragment
    {
        public required long Id { get; set; }
        public required long BookFileId { get; set; }
        public required int Position { get; set; }
        public required DateTime CreatedAt { get; set; }
        [MaxLength(2048)]
        public required string StorageUrl { get; set; }
        public required int StartPos { get; set; }
        public required int EndPos { get; set; }
        public BookFile BookFile { get; set; } = null!;
    }
}
