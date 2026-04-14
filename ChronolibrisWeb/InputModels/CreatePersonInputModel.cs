using System.ComponentModel.DataAnnotations;

namespace ChronolibrisWeb.InputModels
{
    public class CreatePersonInputModel
    {
        [MaxLength(256)]
        [Required]
        public required string Name { get; init; }
        [MaxLength(5000)]
        [Required]
        public required string Description { get; init; }

    }
}
