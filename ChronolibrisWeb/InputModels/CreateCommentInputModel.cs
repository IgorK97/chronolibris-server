using System.ComponentModel.DataAnnotations;

namespace ChronolibrisWeb.InputModels
{
    public record CreateCommentInputModel(
         long BookId,
         [property: MaxLength(5000)]
         string Text,
         long? ParentCommentId = null
     );
}
