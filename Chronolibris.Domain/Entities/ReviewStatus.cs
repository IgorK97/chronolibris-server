//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Chronolibris.Domain.Entities
//{

//    public static class ReviewStatusIds
//    {
//        public const long Pending = 1;
//        public const long Published = 2;
//        public const long Rejected = 3;
//        public const long Deleted = 4;
//    }
//    public class ReviewStatus
//    {
//        public long Id { get; set; }
//        [MaxLength(50)]
//        public required string Name { get; set; }
//        public ICollection<Review> Reviews { get; set; } = new List<Review>();
//    }
//}
