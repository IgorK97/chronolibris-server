//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Chronolibris.Domain.Entities
//{
//    public class Series
//    {
//        public required long Id { get; set; }
//        [MaxLength(500)]
//        public required string Name { get; set; }
//        public long PublisherId { get; set; }
//        public Publisher Publisher { get; set; } = null!;
//        public required DateTime CreatedAt {get;set;}
//        public ICollection<Book> Books { get; set; } = new List<Book>();
        
//    }
//}
