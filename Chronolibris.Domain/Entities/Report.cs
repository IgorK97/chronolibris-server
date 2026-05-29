using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Domain.Entities
{
    public enum ReportTargetType
    {
        Book = 1,
        Review = 2,
        Comment = 3
    };

    public class Report
    {
        public long Id { get; set; }
        public long? BookId { get; set; }    
        public long? ReviewId { get; set; }
        public long? CommentId { get; set; }
        //public long TargetId { get; set; }
        //public long TargetTypeId { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; } = String.Empty;
        //public long StatusId { get; set; }
        public long ReasonTypeId { get; set; }
        public DateTime CreatedAt { get; set; }
        //public DateTime? ModeratedAt { get; set; }
        public long CreatedBy { get; set; }
        public long? ModerationTaskId { get; set; }
        public Book? Book { get; set; }
        public Review? Review { get; set; }
        public Comment? Comment { get; set; }
        public ModerationTask? ModerationTask { get; set; }
        //public long? ModeratedBy { get; set; }
        //public ReportTargetType TargetType { get; set; }
        public ReportReasonType ReasonType { get; set; }
        //public ReportStatus Status { get; set; }
    }
}
