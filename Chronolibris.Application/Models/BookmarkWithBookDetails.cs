using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Application.Models
{
    public class BookmarkWithBookDetails
    {
        public long Id { get; set; }
        public string Xpointer { get; set; } = string.Empty;
        public string Context { get; set; } = string.Empty;
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }

        public long BookFileId { get; set; }
        public string BookFileFormatName { get; set; } = string.Empty;
        public int BookFileFormatId { get; set; }
        public long BookFileStatusId { get; set; }

        public long BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
    }
}
