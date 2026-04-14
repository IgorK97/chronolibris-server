namespace ChronolibrisWeb.InputModels
{
    public record CreateCommentInputModel(
         long BookId,
         string Text,
         long? ParentCommentId = null
     );
}
