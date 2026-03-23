using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Entities
{
    public class Selection
    {
        public required long Id { get; set; }
        [MaxLength(500)]
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required bool IsActive { get; set; }
        public required long CreatedBy { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        //public required int SelectionTypeId { get; set; }
        //public SelectionType SelectionType { get; set; } = null!;
        public ICollection<Book> Books = new List<Book>();
    }
}
