using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Entities
{
    public static class BookFileStatuses
    {
        public static readonly int PENDING = 1;
        public static readonly int UPLOADED = 2;
        public static readonly int PROCESSING = 3;
        public static readonly int COMPLETED = 4;
        public static readonly int FAILED = 5;
    }
    public class BookFileStatus
    {
        public required long Id { get; set; }
        [MaxLength(50)]
        public required string Name { get; set; }
        public ICollection<BookFile> BookFiles { get; set; }

    }
}
