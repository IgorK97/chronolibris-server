using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Models
{
    public class CommentDto
    {
        public required long Id { get; set; }
        public string? Text { get; set; }
        public long UserId { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime? DeletedAt { get; set; }

        public string? UserLogin { get; set; }
        public long? ParentCommentId { get; set; }
        public required long RepliesCount { get; set; }
        public required long LikesCount { get; set; }
        public required long DislikesCount { get; set; }
        public bool? UserVote { get; set; }
        public required bool IsDeleted { get; set; }
        //List<CommentDto>? Replies = null
    }
        
}
