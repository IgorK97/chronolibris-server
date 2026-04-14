using System;
using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Application.Models
{
    public class PublisherDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public long CountryId { get; set; }
        public string? CountryName { get; set; }
    }

    public class CreatePublisherRequest
    {
        [Required]
        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(5000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public long CountryId { get; set; }
    }

    public class UpdatePublisherRequest
    {
        [Required]
        public long Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(5000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public long CountryId { get; set; }
    }
}