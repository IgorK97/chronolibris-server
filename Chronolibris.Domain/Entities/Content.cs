using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Domain.Entities
{
    public class Content
    {
        public required long Id { get; set; }
        [MaxLength(500)]
        public required string Title { get; set; }
        [MaxLength(5000)]
        public required string Description { get; set; }
        public required long CountryId { get; set; }
        public required long ContentTypeId { get; set; }
        public required long LanguageId { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public required DateTime CreatedAt { get; set; }
        public ICollection<BookContent> BookContents { get; set; } = [];
        public Country Country { get; set; } = null!;
        public Language Language { get; set; } = null!;
        public ICollection<ContentParticipation> Participations { get; set; } = [];
        public ICollection<Person> Persons { get; set; } = [];
        public ICollection<Theme> Themes { get; set; }=[];
        public ContentType ContentType { get; set; } = null!;
        public ICollection<Tag> Tags { get; set; } = [];

    }
}
