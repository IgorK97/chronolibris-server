using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Models
{
    public class CountryDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = String.Empty;
    }
}
