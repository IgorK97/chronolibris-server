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
        public required Review Review { get; set; }
        public required long LikesCount { get; set; }
        public required long DislikesCount { get; set; }
        public bool? UserVote { get; set; }
    }
}
