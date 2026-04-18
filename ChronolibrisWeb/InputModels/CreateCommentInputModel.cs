using System.ComponentModel.DataAnnotations;

namespace ChronolibrisWeb.InputModels
{
    public record CreateCommentInputModel( //тоже проверить
         long BookId,
         [MaxLength(5000, ErrorMessage ="Максимальная длина комментария - 5000 символов")]
         //[MinLength(1, ErrorMessage ="Комментарий должен быть написан")]
         string Text,
         long? ParentCommentId = null
     );
}
