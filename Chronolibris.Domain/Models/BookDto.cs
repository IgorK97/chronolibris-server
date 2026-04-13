using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Models
{
    public class BookDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long CountryId { get; set; }
        public string? CountryName { get; set; }
        public long LanguageId { get; set; }
        public string? LanguageName { get; set; }
        public int? Year { get; set; }
        public string? ISBN { get; set; }
        public string? CoverPath { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsReviewable { get; set; }
        public long? PublisherId { get; set; }
        public string? PublisherName { get; set; }
        //public long? SeriesId { get; set; }
        //public string? SeriesName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<string> Authors { get; set; } = new();
        public List<ThemeDto> Themes { get; set; } = new();
    }

}
