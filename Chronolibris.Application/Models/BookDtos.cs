using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Application.Models
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

    public class CreateBookRequest
    {
        [Required]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public long CountryId { get; set; }

        [Required]
        public long LanguageId { get; set; }

        public int? Year { get; set; }
        [MaxLength(17)]
        public string? ISBN { get; set; }
        [MaxLength(2048)]
        public string? FilePath { get; set; }
        [MaxLength(2048)]
        public string? CoverPath { get; set; }
        public bool IsAvailable { get; set; } = true;
        public bool IsReviewable { get; set; } = false;
        public long? PublisherId { get; set; }
        public long? SeriesId { get; set; }
        public List<long> PersonIds { get; set; } = new();
        public List<long> ThemeIds { get; set; } = new();
    }

    public class UpdateBookRequest
    {
        [Required]
        public long Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public long CountryId { get; set; }

        [Required]
        public long LanguageId { get; set; }

        public int? Year { get; set; }
        [MaxLength(17)]
        public string? ISBN { get; set; }
        [MaxLength(2048)]
        public string? FilePath { get; set; }
        [MaxLength(2048)]
        public string? CoverPath { get; set; }
        public bool IsAvailable { get; set; } = true;
        public bool IsReviewable { get; set; } = false;
        public long? PublisherId { get; set; }
        public long? SeriesId { get; set; }
        public List<long> PersonIds { get; set; } = new();
        public List<long> ThemeIds { get; set; } = new();
    }
    public class BookListResponse
    {
        public List<BookDto> Items { get; set; } = new();
        public string? NextCursor { get; set; }
        public string? PrevCursor { get; set; }
        public int TotalCount { get; set; }
        public bool HasMore { get; set; }
    }

    public class BookContentLinkRequest
    {
        [Required]
        public long ContentId { get; set; }
        [Required]
        public long BookId { get; set; }
    }
}