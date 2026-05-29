using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Domain.Entities
{
    public class Format
    {
        [Key]
        public required long Id { get; set; }
        [MaxLength(50)]
        public required string Name { get; set; }
    }
}
