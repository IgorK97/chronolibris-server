using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Domain.Entities
{
    public class Selection
    {
        public required long Id { get; set; }
        [MaxLength(500)]
        public required string Name { get; set; }
        [MaxLength(2000)]
        public required string Description { get; set; }
        public required bool IsActive { get; set; }
        public required long UserId { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        //public required int SelectionTypeId { get; set; }
        //public SelectionType SelectionType { get; set; } = null!;
        public ICollection<Book> Books = new List<Book>();
    }
}
