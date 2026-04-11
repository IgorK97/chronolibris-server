using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.Comments
{
    public record GetCommentRepliesQuery(
        long ParentCommentId,
        long? LastId,
        int Limit,
        long UserId
    ) : IRequest<List<CommentDto>>;
    public class GetCommentRepliesHandler : IRequestHandler<GetCommentRepliesQuery, List<CommentDto>>
    {
        private readonly ICommentRepository _repository;

        public GetCommentRepliesHandler(ICommentRepository repository) => _repository = repository;

        public async Task<List<CommentDto>> Handle(GetCommentRepliesQuery request, CancellationToken ct)
        {
            var replies = await _repository.GetRepliesByParentIdAsync(
                request.ParentCommentId, request.LastId, request.Limit, request.UserId, ct);

            foreach (var comment in replies)
            {
                if (comment.DeletedAt != null)
                {
                    comment.Text = "[Комментарий удален]";
                    comment.UserLogin = "[Недоступно]";
                }
            }

            return replies;
        }
    }
}
