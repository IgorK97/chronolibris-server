using System.ComponentModel.DataAnnotations;

namespace ChronolibrisWeb.InputModels
{
    public record UpdateSelectionInputModel(
            long SelectionId,
            [MaxLength(500)]
            string? Name,
            [MaxLength(2000)]
            string? Description,
            bool? IsActive
        );
}
