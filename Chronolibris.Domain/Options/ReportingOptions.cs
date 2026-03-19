using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Options
{
    public class ReportingOptions
    {
        public const string SectionName = "Reporting";
        public TimeSpan ReportCooldown { get; set; } = TimeSpan.FromDays(14);
    }
}
