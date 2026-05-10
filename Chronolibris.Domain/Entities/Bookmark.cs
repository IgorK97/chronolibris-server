using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Domain.Entities
{
    public class Bookmark
    {
        public required long Id { get; set; }
        public required long BookFileId { get; set; }
        public required long UserId { get; set; }
        [MaxLength(200)]
        public required string Xpointer { get; set; }
        [MaxLength(200)]
        public required string Context { get; set; }
        [MaxLength(1000)]
        public string? Note { get; set; }
        public required DateTime CreatedAt { get; set; }
        public BookFile BookFile { get; set; } = null!;

    }
}
