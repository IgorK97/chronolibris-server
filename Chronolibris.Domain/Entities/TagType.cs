using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Domain.Entities
{
    public class TagType
    {
        [Key]
        public required long Id { get; set; }
        [MaxLength(50)]
        public required string Name { get; set; }
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}
