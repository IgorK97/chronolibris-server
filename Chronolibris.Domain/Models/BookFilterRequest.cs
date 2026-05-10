namespace Chronolibris.Domain.Models
{
    public class BookFilterRequest
    {
        public string? SearchQuery { get; set; }
        public string? AuthorName { get; set; }
        public List<long>? IncludeThemeIds { get; set; }
        public List<long>? ExcludeThemeIds { get; set; }
        public long? PublisherId { get; set; }
        public long? SeriesId { get; set; }
        public long? LanguageId { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public bool? IsAvailable { get; set; }
        public string? Cursor { get; set; }
        public int Limit { get; set; } = 20;
    }
}
