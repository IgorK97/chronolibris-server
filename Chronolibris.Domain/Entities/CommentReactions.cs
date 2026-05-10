namespace Chronolibris.Domain.Entities
{
    public class CommentReactions
    {
        public required long Id { get; set; }
        public required long CommentId { get; set; }
        public required long UserId { get; set; }
        public required short ReactionType { get; set; }

    }
}
