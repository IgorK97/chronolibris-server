using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Entities
{
    public class Comment
    {
        public long Id { get; set; }

        [MaxLength(5000)]
        public required string Text { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required bool IsDeleted { get; set; }
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
