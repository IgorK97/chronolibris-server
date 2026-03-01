using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Entities
{
    public class BookParticipation
    {
        public long Id { get; set; }
        public long PersonId { get; set; }
        public Person Person { get; set; } = null!;
        public long PersonRoleId { get; set; }
        public PersonRole PersonRole { get; set; } = null!;
        public long BookId { get; set; }
        public Book Book { get; set; } = null!;
    }
}
