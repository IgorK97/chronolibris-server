using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Comments
{
    public record RateCommentCommand(long CommentId, long UserId, short Score) : IRequest<CommentDto?>;
}
