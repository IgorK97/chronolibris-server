using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Domain.Entities
{
    public class Tag
    {
        public required long Id { get; set; }
        [MaxLength(500)]
        public required string Name { get; set; }
        public required long TagTypeId { get; set; }
        public long? ParentTagId { get; set; }
        public Tag? ParentTag { get; set; }
        public ICollection<Tag> ChildTags { get; set; } = new List<Tag>();
        public long? RelationTypeId { get; set; }
        public TagRelationType? RelationType { get; set; }
        public TagType TagType { get; set; }
        public ICollection<Content> Contents { get; set; } = new List<Content>();
    }
}
