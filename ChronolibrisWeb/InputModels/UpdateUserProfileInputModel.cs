using System.ComponentModel.DataAnnotations;

namespace ChronolibrisWeb.InputModels
{
    public class UpdateUserProfileInputModel
    {
        [MaxLength(256)]
        public required string FirstName { get; init; }
        [MaxLength(256)]
        public required string LastName { get; init; }
        [MaxLength(256)]
        public string? Email { get; init; }
        [MaxLength(20)]
        public string? PhoneNumber { get; init; }
        [MaxLength(256)]
        public required string UserName { get; init; }

    }
}
