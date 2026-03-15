using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Entities
{
    public class Content
    {
        public required long Id { get; set; }
        [MaxLength(500)]
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required long CountryId { get; set; }
        public required long ContentTypeId { get; set; }
        public required long LanguageId { get; set; }
        public int? Year { get; set; }
        public long? ParentContentId { get; set; }
        public Content? ParentContent { get; set; }

        public int? Position { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<BookContent> BookContents { get; set; } = [];
        public Country Country { get; set; } = null!;
        public Language Language { get; set; } = null!;
        public ICollection<ContentParticipation> Participations { get; set; } = [];
        public ICollection<Person> Persons { get; set; } = [];
        public ICollection<Theme> Themes { get; set; }=[];
        public ContentType ContentType { get; set; } = null!;
        public ICollection<Tag> Tags { get; set; } = null!;

    }
}
