using System.ComponentModel.DataAnnotations;

namespace ChronolibrisWeb.InputModels
{
    public class ChangePasswordInputModel
    {
        [MaxLength(128)]
        public required string CurrentPassword { get; init; }
        [MaxLength(128)]
        public required string NewPassword { get; init; }
    }
}
