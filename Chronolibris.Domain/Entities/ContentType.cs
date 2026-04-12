using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Entities
{
    public enum ContentNature
    {
        Unknown = 0,
        Document = 1,
        Work = 2,
        Analysis = 3,
    }
    public class ContentType
    {
        public long Id { get; set; }
        [MaxLength(100)]
        public required string Name { get; set; }
        public ContentNature Nature { get; set; } = ContentNature.Unknown;
        public ICollection<Content> Contents { get; set; } = [];
    }
}
