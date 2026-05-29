using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Domain.Entities
{
    public class ModerationTask
    {
        public long Id { get; set; }
        public long? BookId { get; set; }
        public long? CommentId { get; set; }
        public long? ReviewId { get; set; }
        //public long TargetId { get; set; }
        //public long TargetTypeId { get; set; }
        public long ModeratedBy { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public long StatusId { get; set; }
        //public int CheckNumber { get; set; }
        //public long ReasonTypeId { get; set; }
        [MaxLength(5000)]
        [Required]
        public string CommentText { get; set; } = String.Empty;
        public ICollection<Report> Reports { get; set; } = [];
        public ReportStatus Status { get; set; } = null!;
        public Book? Book { get; set; }
        public Review? Review { get; set; }
        public Comment? Comment { get; set; }
        //public ReportReasonType ReasonType { get; set; } = null!;
        //public ReportTargetType TargetType { get; set; } = null!;

    }
}
