using Chronolibris.Application.Requests.Reports;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.Reports
{
    public class CreateModerationTaskCommandHandler
        :IRequestHandler<CreateModerationTaskCommand, CreateModerationTaskResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateModerationTaskCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<CreateModerationTaskResponse> Handle(
            CreateModerationTaskCommand request, CancellationToken token)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            //IAsyncDisposable - можно трай не делать, так как
            //технически при использовании юзинг само откатит назад
            //но я оставлю
            //try
            //{
            var lastTask = await _unitOfWork.ModerationTasks.GetLastTaskAsync(request.TargetId, request.TargetTypeId, token);

            if (lastTask != null && lastTask.StatusId == 2)
            {
                throw new ChronolibrisException("Для данного контента уже существует активная задача модерации", ErrorType.Conflict);
            }

            //var checkNumber = (lastTask?.CheckNumber ?? 0) + 1;
            long? bookId = null;
            long? commentId = null;
            long? reviewId = null;

            if (request.TargetTypeId == 1)
            {
                bookId = request.TargetId;
            }
            else if (request.TargetTypeId == 2)
            {
                reviewId = request.TargetId;
            }
            else if (request.TargetTypeId == 3)
            {
                commentId = request.TargetId;
            }
            else throw new ChronolibrisException("Неверный тип контента", ErrorType.Validation);

            var newTask = new ModerationTask
            {
                BookId = bookId,
                CommentId = commentId,
                ReviewId = reviewId,
                ModeratedBy = request.ModeratorId,
                StartedAt = DateTime.UtcNow,
                StatusId = 2,
                CommentText = "",
                //CheckNumber = checkNumber,
                //ReasonTypeId = request.ReportTypeId,
            };
            //пока так оставлю, по идее, можно было бы и обычнм адд асинк, но тогда нужно как-то читать сообщение об ошибке где-то
            //и конвертировать его в читаемый вид
            var newTaskId = await _unitOfWork.ModerationTasks.TryCreateActiveTaskAsync(newTask, token);
            if (newTaskId == null || newTaskId == 0)
                throw new ChronolibrisException("Для данного контента уже существует активная задача модерации",
                    ErrorType.Conflict);
            //await _unitOfWork.SaveChangesAsync(token);

            await _unitOfWork.Reports.AttachReportsToTaskAsync(
                (long)newTaskId,
                request.TargetId,
                request.TargetTypeId,
                token);
            await _unitOfWork.SaveChangesAsync(token);
            await transaction.CommitAsync(token);

            return new CreateModerationTaskResponse
            {
                Id = newTask.Id,
                TaskCreatedAt = newTask.StartedAt,
                TaskStatusId = newTask.StatusId,
            };
            //}
            //catch
            //{
            //    await transaction.RollbackAsync();
            //    throw;
            //}

        }
    }

}
