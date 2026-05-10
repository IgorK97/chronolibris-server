namespace Chronolibris.Application.Models
{
    public class ShelfDetails
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public required long ShelfType { get; set; }
    }

}
