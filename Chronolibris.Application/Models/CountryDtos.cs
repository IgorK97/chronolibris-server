using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Application.Models
{
    //public class CountryDto
    //{
    //    public long Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //}

    public class CreateCountryRequest
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateCountryRequest
    {
        [Required]
        public long Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
    }
}