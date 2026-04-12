namespace ChronolibrisWeb.InputModels
{
    public record CreateReportInputModel(long TargetId, long TargetTypeId, 
        long ReasonTypeId, string Description);

}
