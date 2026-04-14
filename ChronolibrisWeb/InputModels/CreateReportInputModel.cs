using System.ComponentModel.DataAnnotations;

namespace ChronolibrisWeb.InputModels
{
    public record CreateReportInputModel(long TargetId, long TargetTypeId, 
        long ReasonTypeId,
        [MaxLength(2000)]
        [MinLength(20)]
        string Description);

}
