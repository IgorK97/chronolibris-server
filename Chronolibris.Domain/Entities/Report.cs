using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Entities
{
    public class Report
    {
        public long Id { get; set; }
        public long TargetId { get; set; }
        public long TargetTypeId { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        //public long StatusId { get; set; }
        public long ReasonTypeId { get; set; }
        public DateTime CreatedAt { get; set; }
        //public DateTime? ModeratedAt { get; set; }
        public long CreatedBy { get; set; }
        public long? ModerationTaskId { get; set; }
        public ModerationTask? ModerationTask { get; set; }
        //public long? ModeratedBy { get; set; }
        public ReportTargetType TargetType { get; set; }
        public ReportReasonType ReasonType { get; set; }
        //public ReportStatus Status { get; set; }
    }
}
