using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Domain.Entities
{
    public class Comment
    {
        public long Id { get; set; }

        [MaxLength(5000)]
        public required string Text { get; set; }
        public required DateTime CreatedAt { get; set; }
        //public required bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public long UserId { get; set; }
        public long BookId { get; set; }
        public Book Book { get; set; } = null!;
        public long? ParentCommentId { get; set; }
        public Comment? ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
        public ICollection<CommentReactions> CommentReactions { get; set; } = new List<CommentReactions>();

    }
}
