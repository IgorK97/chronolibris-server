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
    public record RateCommentCommand(long CommentId, long UserId, short Score) : IRequest<CommentDto?>;
}
