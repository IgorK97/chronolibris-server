using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Application.Models
{


    public class CreateThemeInputModel
    {
        [MinLength(1)]
        [MaxLength(500)]
        public string Name { get; set; } = string.Empty;

        public long? ParentThemeId { get; set; }
    }

    public class UpdateThemeInputModel
    {
        public long Id { get; set; }

        [MinLength(1)]
        [MaxLength(500)]
        public string Name { get; set; } = string.Empty;

        public long? ParentThemeId { get; set; }
    }
}