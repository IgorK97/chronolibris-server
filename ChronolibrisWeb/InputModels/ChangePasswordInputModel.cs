namespace ChronolibrisWeb.InputModels
{
    public class ChangePasswordInputModel
    {
        public required string CurrentPassword { get; init; }
        public required string NewPassword { get; init; }
    }
}
