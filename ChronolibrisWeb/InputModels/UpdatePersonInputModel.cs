using System.ComponentModel.DataAnnotations;

namespace ChronolibrisWeb.InputModels
{
    public class UpdatePersonInputModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage ="Имя персоналии обязательно")]
        public required string Name { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Описание персоналии обязательно")]
        public required string Description { get; set; }


    }
}
