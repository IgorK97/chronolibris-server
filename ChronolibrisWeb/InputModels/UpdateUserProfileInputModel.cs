namespace ChronolibrisWeb.InputModels
{
    public class UpdateUserProfileInputModel
    {
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public string? Email { get; init; }
        public string? PhoneNumber { get; init; }
        public required string UserName { get; init; }

    }
}
