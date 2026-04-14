using System.ComponentModel.DataAnnotations;

namespace ChronolibrisWeb.InputModels
{
    public class UpdateReviewInputModel
    {
        [MaxLength(5000)]
        public string? ReviewText { get; init; }
        public short Score { get; init; }

    }
}
