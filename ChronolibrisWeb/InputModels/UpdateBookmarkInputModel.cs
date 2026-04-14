using System.ComponentModel.DataAnnotations;

namespace ChronolibrisWeb.InputModels
{
    public class UpdateBookmarkInputModel
    {
        [MaxLength(1000)]
        public string? Note { get; init; }
    }
}
