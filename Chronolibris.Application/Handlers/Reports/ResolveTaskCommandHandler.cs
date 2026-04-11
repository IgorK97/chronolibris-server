using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests.Reports;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.Reports
{
    public class ResolveTaskCommandHandler :IRequestHandler<ResolveTaskCommand,
        TaskResolutionResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ResolveTaskCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TaskResolutionResponse> Handle(
            ResolveTaskCommand command, CancellationToken token)
        {
            var task = await _unitOfWork.ModerationTasks
                .GetByIdAsync(command.TaskId, token);

            if (task is null ||
                task.ModeratedBy != command.ModeratorId ||
                task.StatusId!=2)
                return new TaskResolutionResponse
                {
                    Success = false,
                };
            var now = DateTime.UtcNow;

            await using var transaction = await _unitOfWork.BeginTransactionAsync(token);
            try
            {
                task.Comment = command.Comment;
                task.StatusId = command.Resolution ? 3 : 4;
                task.ResolvedAt = now;
                if (command.Resolution)
                {
                    switch (task.TargetTypeId)
                    {
                        case 3:
                            {
                                var comment = await _unitOfWork.Comments.GetByIdAsync(task.TargetId, token);
                                if(comment is not null && !comment.IsDeleted)
                                {
                                    comment.IsDeleted = true;
                                    comment.DeletedAt = now;
                                }
                                break;
                            }
                        case 2:
                            {
                                var review = await _unitOfWork.Reviews.GetByIdAsync(task.TargetId, token);
                                if(review is not null && !review.IsDeleted)
                                {
                                    review.IsDeleted = true;
                                    review.DeletedAt = now;
                                    //review.ModeratedAt = now;
                                }
                                break;
                            }
                        case 1:
                            {
                                var book = await _unitOfWork.Books.GetByIdAsync(task.TargetId, token);
                                if(book is not null && book.IsAvailable)
                                {
                                    book.IsAvailable = false;
                                    book.UpdatedAt = now;
                                }
                                break;
                            }
                    }
                    
                }
                await _unitOfWork.SaveChangesAsync(token);
                await transaction.CommitAsync(token);
                return new TaskResolutionResponse
                {
                    Success = true,
                    TaskResolvedAt = now,
                    TaskStatusId = task.StatusId
                };
            }
            catch
            {
                await transaction.RollbackAsync(token);
                throw;
            }
                
        }
    }
}
