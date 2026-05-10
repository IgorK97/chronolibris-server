namespace Chronolibris.Application.Models
{
    public class BookmarkDetails
    {
        public long Id { get; set; }
        public string Xpointer { get; set; }
        public string Context { get; set; }
        public string? Note { get; set; }
        public required long BookFileId { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}
