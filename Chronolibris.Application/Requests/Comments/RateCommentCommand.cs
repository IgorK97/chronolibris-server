using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Comments
{
    public class RateCommentCommand : IRequest<CommentDto?>
    {
        public long CommentId { get; init; }
        public long UserId { get; init; }
        public short Score { get; init; }
    }
}
