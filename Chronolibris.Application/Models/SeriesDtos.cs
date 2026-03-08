// File: Chronolibris.Application.Models.SeriesDtos.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Application.Models
{
    public class SeriesDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public long PublisherId { get; set; }
        public string? PublisherName { get; set; }
        public DateTime CreatedAt { get; set; }
        public int BooksCount { get; set; }
    }

    public class CreateSeriesRequest
    {
        [Required]
        [MaxLength(500)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public long PublisherId { get; set; }
    }

    public class UpdateSeriesRequest
    {
        [Required]
        public long Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public long PublisherId { get; set; }
    }

    public class DeleteSeriesRequest
    {
        [Required]
        public long Id { get; set; }
    }
}