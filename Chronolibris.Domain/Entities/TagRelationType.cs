using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Domain.Entities
{
    public class TagRelationType
    {
        public required long Id { get; set; }
        [MaxLength(100)]
        public required string Name { get; set; }        
        [MaxLength(200)]
        public string? Description { get; set; }        
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}
