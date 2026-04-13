using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Chronolibris.Application.Requests.Reviews
{
    public record UpdateReviewCommand(long ReviewId, long UserId, short Score) : IRequest<Unit>;
}
