using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Reports.Commands;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Reports.Handlers
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
            try
            {
                var task = await _unitOfWork.Reports.CreateModerationTaskWithReportsAsync(
                    request.TargetId,
                    request.TargetTypeId,
                    request.ReportTypeId,
                    request.ModeratorId,
                    transaction);

                await transaction.CommitAsync();
                return new CreateModerationTaskResponse
                {
                    Id = task.Id,
                    TaskCreatedAt = task.StartedAt,
                    TaskStatusId = task.StatusId,
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
