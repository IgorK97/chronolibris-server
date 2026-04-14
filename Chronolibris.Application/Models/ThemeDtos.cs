using System;
using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Application.Models
{


    public class CreateThemeInputModel
    {
        [Required]
        [MaxLength(500)]
        public string Name { get; set; } = string.Empty;

        public long? ParentThemeId { get; set; }
    }

    public class UpdateThemeInputModel
    {
        [Required]
        public long Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Name { get; set; } = string.Empty;

        public long? ParentThemeId { get; set; }
    }
}