using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Chronolibris.Application.Requests.Reviews
{
    public class CreateReviewCommand : IRequest<long>
    {
        public long BookId { get; init; }
        public long UserId { get; init; }
        public string? ReviewText { get; init; }
        public short Score { get; init; }
    }
}
