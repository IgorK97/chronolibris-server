using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Application.Models
{

    public class CreateLanguageRequest
    {
        [MinLength(1)]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateLanguageRequest
    {
        public long Id { get; set; }

        [MinLength(1)]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}