namespace Chronolibris.Domain.Models.Search
{
    public class BookSearchResult
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CoverPath { get; set; } = string.Empty;
        public int? Year { get; set; }
        public double? AverageRating { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsReviewable { get; set; }
        public required List<string> Authors { get; set; }
    }
}
