namespace Chronolibris.Domain.Models
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; init; } = [];
        public int Limit { get; set; }
        public bool HasNext { get; set; }
        public long? LastId { get; set; }
    }
}
