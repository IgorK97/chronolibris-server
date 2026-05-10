using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Application.Models
{
    public class PublisherDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreatePublisherRequest
    {
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(5000)]
        public string Description { get; set; } = string.Empty;
    }

    public class UpdatePublisherRequest
    {
        public long Id { get; set; }

        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(5000)]
        public string Description { get; set; } = string.Empty;
    }
}