namespace Chronolibris.Domain.Entities
{
    public class ReviewReactions
    {
        public required long Id { get; set; }
        public required long ReviewId { get; set; }
        public required long UserId { get; set; }
        public required short ReactionType { get; set; }
    }
}
