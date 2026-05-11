using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Domain.Entities
{
    public enum PersonRoleKind
    {
        Content = 1,
        Book = 2,
        Both = 3
    }
    public class PersonRole
    {
        public required long Id { get; set; }
        [MaxLength(50)]
        public required string Name { get; set; }
        public PersonRoleKind Kind { get; set; }
    }
}
