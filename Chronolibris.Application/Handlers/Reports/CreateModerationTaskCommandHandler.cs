using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            //IAsyncDisposable - как оказалось, можно трай не делать, так как
            //технически при использовании юзинг само откатит назад
            //но я оставлю - так нагляднее и надежнее
            try
            {
                var lastTask = await _unitOfWork.ModerationTasks.GetLastTaskAsync(request.TargetId, request.TargetTypeId, token);

                if (lastTask != null && lastTask.StatusId == 2 && lastTask.ReasonTypeId == request.ReportTypeId)
                {
                    throw new ChronolibrisException("Для данного контента уже существует активная задача модерации", ErrorType.Conflict);
                }

                var checkNumber = (lastTask?.CheckNumber ?? 0) + 1;

                var newTask = new ModerationTask
                {
                    TargetId = request.TargetId,
                    TargetTypeId = request.TargetTypeId,
                    ModeratedBy = request.ModeratorId,
                    StartedAt = DateTime.UtcNow,
                    StatusId = 2,
                    Comment = "",
                    CheckNumber = checkNumber,
                    ReasonTypeId = request.ReportTypeId,
                };

                var newTaskId = await _unitOfWork.ModerationTasks.TryCreateActiveTaskAsync(newTask, token);
                if (newTaskId == null || newTaskId == 0)
                    throw new ChronolibrisException("Для данного контента уже существует активная задача модерации",
                        ErrorType.Conflict);
                //await _unitOfWork.SaveChangesAsync(token);

                await _unitOfWork.Reports.AttachReportsToTaskAsync(
                    (long)newTaskId,
                    request.TargetId,
                    request.TargetTypeId,
                    request.ReportTypeId,
                    token);
                await _unitOfWork.SaveChangesAsync(token);
                await transaction.CommitAsync(token);

                return new CreateModerationTaskResponse
                {
                    Id = newTask.Id,
                    TaskCreatedAt = newTask.StartedAt,
                    TaskStatusId = newTask.StatusId,
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }
    }

}
