namespace Chronolibris.Application.Models
{
    public class RegistrationResult
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? Message { get; set; }
        public required long UserId { get; init; }
    }
}
