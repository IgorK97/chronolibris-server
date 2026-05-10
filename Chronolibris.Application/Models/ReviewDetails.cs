namespace Chronolibris.Application.Models
{
    public class ReviewDetails
    {
        public required long Id { get; set; }
        public required string UserName { get; set; }
        public string? Text { get; set; }
        public required short Score { get; set; }
        public required long LikesCount { get; set; }
        public required long DislikesCount { get; set; }
        public required DateTime CreatedAt { get; set; }
        public bool? UserVote { get; set; }
    }
}
