namespace Chronolibris.Domain.Models
{
    public class ContentFilterRequest
    {
        public string? SearchQuery { get; set; }
        public long? LastId { get; set; }
        public int Limit { get; set; } = 20;
    }
}
