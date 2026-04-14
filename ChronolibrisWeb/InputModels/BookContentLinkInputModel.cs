using System.ComponentModel.DataAnnotations;

namespace ChronolibrisWeb.InputModels
{
    public class BookContentLinkInputModel
    {
        [Required]
        public long ContentId { get; set; }
        [Required]
        public long BookId { get; set; }
    }
}
