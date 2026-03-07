using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Models
{
    public class FtsConfigurationDto
    {
        public long ConfigOid { get; set; }
        public string ConfigName { get; set; } = string.Empty;
    }
}
