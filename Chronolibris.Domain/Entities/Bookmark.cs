using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Entities
{
    public class Bookmark
    {
        public required long Id { get; set; }
        public required long BookFileId { get; set; }
        public required long UserId { get; set; }
        public required int ParaIndex { get; set; }
        [MaxLength(1000)]
        public string? Note { get; set; }
        public required DateTime CreatedAt { get; set; }
        public BookFile BookFile { get; set; } = null!;

    }
}
