using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.Comments
{
    public record GetBookCommentsQuery(
        long BookId,
        long? LastId,
        int Limit,
        long UserId
    ) : IRequest<List<CommentDto>>;
    public class GetBookCommentsHandler : IRequestHandler<GetBookCommentsQuery, List<CommentDto>>
    {
        private readonly ICommentRepository _repository;

        public GetBookCommentsHandler(ICommentRepository repository) => _repository = repository;

        public async Task<List<CommentDto>> Handle(GetBookCommentsQuery request, CancellationToken ct)
        {
            var comments = await _repository.GetRootCommentsByBookIdAsync(
                request.BookId, request.LastId, request.Limit, request.UserId, ct);

            foreach(var comment in comments)
            {
                if(comment.DeletedAt != null)
                {
                    comment.Text = "[Комментарий удален]";
                    comment.UserLogin = "[Недоступно]";
                }    
            }

            return comments;
        }


    }
}
