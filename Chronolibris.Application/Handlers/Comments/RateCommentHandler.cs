using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.Comments;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.Comments
{
    public class RateCommentHandler : IRequestHandler<RateCommentCommand, CommentDto?>
    {

        private readonly IUnitOfWork _unitOfWork;

        public RateCommentHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CommentDto?> Handle(RateCommentCommand request, CancellationToken cancellationToken)
        {
            if (request.Score != 1 && request.Score != -1)
                throw new Exception("Неверная оценка");

            var comment = await _unitOfWork.Comments.GetByIdWithVotesAsync(request.CommentId, request.UserId, cancellationToken);
            if (comment == null)
                return null;

            //Потом разблокировать
            //if(review.Review.UserId == request.UserId) 
            //    return null;


            var rating = await _unitOfWork.CommentReactions.GetCommentReactionByUserIdAsync(request.CommentId,
                request.UserId, cancellationToken);

            if (rating is null)
            {
                rating = new CommentReactions
                {
                    Id = 0,
                    CommentId = request.CommentId,
                    ReactionType = request.Score,
                    UserId = request.UserId,
                };
                await _unitOfWork.CommentReactions.AddAsync(rating, cancellationToken);

            }
            else
            {
                rating.ReactionType = request.Score == rating.ReactionType ? (short)0 : request.Score;
            }



            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return comment;

        }



    }
}
