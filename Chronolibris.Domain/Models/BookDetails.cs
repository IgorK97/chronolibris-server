using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Models
{

    public class BookDetails
    {
        public long Id { get; set; }
        public required string Title { get; set; }
        public int? Year { get; set; }
        public required string Description { get; set; }
        public string? ISBN { get; set; }
        public string? Bbk { get; set; }
        public string? Udk { get; set; }
        public string? Source { get; set; }
        public required decimal AverageRating { get; set; }
        public required long RatingsCount { get; set; }
        public required long CommentsCount { get; set; }
        public required long ReviewsCount { get; set; }
        public required decimal UserRating { get; set; }
        public string? CoverUri { get; set; }
        public required bool IsAvailable { get; set; }
        public required bool IsFavorite { get; set; }
        public required bool IsRead { get; set; }
        public required bool IsReviewable { get; set; }
        public PublisherDetails? Publisher { get; set; }
        public CountryDto Country { get; set; } = new();
        public LanguageDto Language { get; set; } = new();
        public IEnumerable<BookPersonGroupDetails> Participants { get; set; } = [];
        public IEnumerable<ThemeDetails> Themes { get; set; } = [];
        public required IEnumerable<TagShortDetails> Tags { get; set; } = [];
    }

    public record TagShortDetails(long Id, string Name, long TagTypeId, string TagTypeName);
}
