using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Domain.Entities
{
    public static class BookFileStatuses
    {
        public static readonly int PENDING = 1;
        public static readonly int UPLOADED = 2;
        public static readonly int PROCESSING = 3;
        public static readonly int COMPLETED = 4;
        public static readonly int FAILED = 5;
        public static readonly int ARCHIVE = 6;
        public static readonly int DELETED = 7;
    }
    public class BookFileStatus
    {
        public required long Id { get; set; }
        [MaxLength(50)]
        public required string Name { get; set; }
        public ICollection<BookFile> BookFiles { get; set; }

    }
}
