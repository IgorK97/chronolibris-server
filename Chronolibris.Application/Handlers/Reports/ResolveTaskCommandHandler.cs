using Chronolibris.Application.Requests.Reports;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
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


            //await using var transaction = await _unitOfWork.BeginTransactionAsync(token);
            //try
            //{
                var task = await _unitOfWork.ModerationTasks
                .GetByIdAsync(command.TaskId, token);

                if (task is null)
                    throw new ChronolibrisException("Задача модерации не найдена", ErrorType.NotFound);

                if (task.ModeratedBy != command.ModeratorId)
                    throw new ChronolibrisException("Эта задача назначена на другого модератора", ErrorType.Forbidden);

                if (task.StatusId != 2)
                    throw new ChronolibrisException("Задача должна быть в статусе 'В работе'", ErrorType.Validation);

                var now = DateTime.UtcNow;
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
                                if(comment is not null && comment.DeletedAt == null)
                                {
                                    //comment.IsDeleted = true;
                                    comment.DeletedAt = now;
                                }
                                break;
                            }
                        case 2:
                            {
                                var review = await _unitOfWork.Reviews.GetByIdAsync(task.TargetId, token);
                                if(review is not null && review.DeletedAt == null)
                                {
                                    //review.IsDeleted = true;
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
                //await transaction.CommitAsync(token);
                return new TaskResolutionResponse
                {
                    Success = true,
                    TaskResolvedAt = now,
                    TaskStatusId = task.StatusId
                };
            //}
            //catch
            //{
            //    await transaction.RollbackAsync(token);
            //    throw;
            //}
                
        }
    }
}
