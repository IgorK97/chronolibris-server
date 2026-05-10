namespace Chronolibris.Domain.Models
{
    public class BookPersonGroupDetails
    {
        public required long Role { get; set; }
        public IEnumerable<PersonDetails> Persons { get; set; } = [];
    }
}
