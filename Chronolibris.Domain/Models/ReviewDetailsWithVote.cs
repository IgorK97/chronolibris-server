using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;

namespace Chronolibris.Domain.Models
{
    public class ReviewDetailsWithVote
    {
        public Review Review { get; set; }
        public string UserName { get; set; } = String.Empty;
        public long LikesCount { get; set; }
        public long DislikesCount { get; set; }
        public bool? UserVote { get; set; }
        public bool IsDeleted { get; set; }
    }
}
