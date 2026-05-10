using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Domain.Entities
{
    public class ReportStatus
    {
        public long Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        public ICollection<ModerationTask> Tasks { get; set; } = [];
    }
}
