using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Entities
{
    public class Tag
    {
        public required long Id { get; set; }
        [MaxLength(500)]
        public required string Name { get; set; }
        public required long TagTypeId { get; set; }
        //public long? ParentTagId { get; set; }
        public TagType TagType { get; set; }
        public ICollection<Content> Contents { get; set; } = new List<Content>();
    }
}
