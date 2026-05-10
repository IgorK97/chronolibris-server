using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Application.Models
{
    public class CreateCountryRequest
    {
        [MinLength(1)]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateCountryRequest
    {
        [MinLength(1)]
        public long Id { get; set; }

        [MinLength(1)]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
    }
}