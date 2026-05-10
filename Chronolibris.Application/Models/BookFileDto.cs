namespace Chronolibris.Application.Models
{
    public class BookFileDto
    {
        public long Id { get; set; }
        public long BookId { get; set; }
        public int FormatId { get; set; }
        public string? FormatName { get; set; }
        public string StorageUrl { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public long StoredSizeBytes { get; set; }
        public bool IsReadable { get; set; }
        public bool? HistoricalText { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public long BookFileStatusId { get; set; }
        public string? BookFileStatusName { get; set; }
    }
}