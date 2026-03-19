using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Reports.Commands;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Reports.Handlers
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
            task.StatusId = command.Resolution ? 3 : 4;

            task.ResolvedAt = now;

            await _unitOfWork.SaveChangesAsync();

            return new TaskResolutionResponse
            {
                Success = true,
                TaskResolvedAt = now,
                TaskStatusId = task.StatusId
            };
                
        }
    }
}
