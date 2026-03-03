using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Chronolibris.Application.Requests
{
    public class DeleteReviewCommand : IRequest<Unit>
    {
        public long ReviewId { get; set; }
        public long UserId { get; set; }
    }
}
