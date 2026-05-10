namespace Chronolibris.Domain.Models
{
    public class BookListItem
    {
        public required long Id { get; set; }
        public required string Title { get; set; }
        public string? CoverUri { get; set; }
        public required decimal AverageRating { get; set; }
        public required long RatingsCount { get; set; }
        public required bool IsFavorite { get; set; }
        public required bool IsRead { get; set; }
        public required bool IsReviewable { get; set; }
        public IEnumerable<string> Authors { get; set; } = [];
    }
}
