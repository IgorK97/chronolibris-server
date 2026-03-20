using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Entities
{
    public class Review
    {
        public required long Id { get; set; }
        public required long UserId { get; set; }
        public required long BookId { get; set; }
        [MinLength(120)]
        [MaxLength(5000)]
        public string? ReviewText { get; set; }
        public required short Score { get; set; }
        public required long ReviewStatusId { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        //public DateTime? ModeratedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public ReviewStatus ReviewStatus { get; set; } = null!;
        public ICollection<ReviewReactions> ReviewsRatings { get; set; } = new List<ReviewReactions>();
    }
}
