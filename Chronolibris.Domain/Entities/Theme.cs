using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chronolibris.Domain.Entities
{
    public class Theme
    {
        [Key]
        public required long Id { get; set; }
        [MaxLength(500)]
        [Required]
        public string Name { get; set; } = null!;
        public long? ParentThemeId { get; set; }
        [ForeignKey("ParentThemeId")]
        public Theme? ParentTheme { get; set; }
        public ICollection<Theme>? SubThemes { get; set; }
        public ICollection<Content>? Contents { get; set; }
    }
}
