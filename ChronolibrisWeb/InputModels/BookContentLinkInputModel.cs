using System.ComponentModel.DataAnnotations;

namespace ChronolibrisWeb.InputModels
{
    public class BookContentLinkInputModel
    {
        public long ContentId { get; set; }
        public long BookId { get; set; }
    }
}
