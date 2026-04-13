using System.ComponentModel.DataAnnotations;

namespace ChronolibrisWeb.InputModels
{
    public class CreateReviewInputModel
    {
        [Required(ErrorMessage = "Не указан идентификатор книги")]
        public long BookId { get; init; }
        [MinLength(120, ErrorMessage = "Текст отзыва должен быть не менее 120 символов")]
        [MaxLength(5000, ErrorMessage = "Текст отзыва не может превышать 5000 символов")]
        public string? ReviewText { get; init; }
        [Required(ErrorMessage = "Не указана оценка")]
        [Range(1, 5, ErrorMessage = "Оценка должна быть от 1 до 5")]
        public short Score { get; init; }
    }
}
