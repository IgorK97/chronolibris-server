using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Entities
{
    public class ReportReasonType
    {
        public long Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        public ICollection<Report> Reports { get; set; } = [];
        public ICollection<ModerationTask> ModerationTasks { get; set; } = [];
    }
}
