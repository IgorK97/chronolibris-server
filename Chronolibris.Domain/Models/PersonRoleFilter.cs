namespace Chronolibris.Domain.Models
{
    public class PersonRoleFilter
    {
        public long RoleId { get; set; }

        public List<long> PersonIds { get; set; } = [];
        public List<string>? PersonNames { get; set; } = [];
    }
}
