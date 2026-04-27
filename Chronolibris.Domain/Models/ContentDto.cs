using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Models
{
    public class ContentDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long CountryId { get; set; }
        public string? CountryName { get; set; }
        public long ContentTypeId { get; set; }
        public string? ContentType { get; set; }
        public long LanguageId { get; set; }
        public string? LanguageName { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Authors { get; set; } = new();
        public List<PersonRoleFilter> Participants { get; set; } = new();
        public List<ThemeDto> Themes { get; set; } = new();
        public int BooksCount { get; set; }
    }

}
