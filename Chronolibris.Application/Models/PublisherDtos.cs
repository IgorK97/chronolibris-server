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
        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(5000)]
        public string Description { get; set; } = string.Empty;
    }

    public class UpdatePublisherRequest
    {
        public long Id { get; set; }

        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(5000)]
        public string Description { get; set; } = string.Empty;
    }
}