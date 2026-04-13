using System.ComponentModel.DataAnnotations;
using Chronolibris.Domain.Models;

namespace Chronolibris.Application.Models
{
 
    public class BookListResponse
    {
        public List<BookDto> Items { get; set; } = new();
        public string? NextCursor { get; set; }
        public string? PrevCursor { get; set; }
        public int TotalCount { get; set; }
        public bool HasMore { get; set; }
    }


}