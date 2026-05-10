using System.ComponentModel.DataAnnotations;

namespace Chronolibris.Domain.Entities
{
    public class ShelfType
    {
        public long Id { get; set; }
        //public string Name { get; set; }
        [MaxLength(50)]
        public string Code { get; set; }

    }
}
