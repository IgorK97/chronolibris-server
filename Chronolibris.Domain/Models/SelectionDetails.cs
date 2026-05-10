namespace Chronolibris.Application.Models
{

    public class SelectionDetails
    {
        public required long Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt{get;set;}
        public long? BooksCount { get; set; }
        public bool IsActive { get; set; }
        
    }
}
