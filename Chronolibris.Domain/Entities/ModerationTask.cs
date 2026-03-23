using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Entities
{
    public class ModerationTask
    {
        public long Id { get; set; }
        public long TargetId { get; set; }
        public long TargetTypeId { get; set; }
        public long ModeratedBy { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public long StatusId { get; set; }
        public long ReasonTypeId { get; set; }
        [MaxLength(1000)]
        public string? Comment { get; set; }
        public ICollection<Report> Reports { get; set; }
        public ReportStatus Status { get; set; }
        public ReportReasonType ReasonType { get; set; }
        public ReportTargetType TargetType { get; set; }

    }
}
