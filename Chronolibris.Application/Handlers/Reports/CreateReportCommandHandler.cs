using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Chronolibris.Application.Requests.Reports;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Options;
using FluentValidation;
using MediatR;

namespace Chronolibris.Application.Handlers.Reports
{
    public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, CreateReportResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ReportingOptions _options;

        public CreateReportCommandHandler(
            IUnitOfWork unitOfWork,
            ReportingOptions options)
        {
            _unitOfWork = unitOfWork;
            _options = options;
        }

        public async Task<CreateReportResult> Handle(
            CreateReportCommand request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            var cooldownThreshold = now - _options.ReportCooldown;

            var isOnCooldown = await _unitOfWork.Reports.GetLastUserReport(request.UserId,
                request.TargetTypeId, request.TargetId, request.ReasonTypeId);

            if (isOnCooldown is not null && isOnCooldown.CreatedAt >= cooldownThreshold)
                return new CreateReportResult(
                    false, $"Вы уже отправляли подобную жалобу. Жалобы можно отправлять" +
                    $"не ранее, чем через {_options.ReportCooldown.TotalDays} дн.");

            var activeTask = await _unitOfWork.ModerationTasks.GetActiveByTarget(request.TargetId,
                request.TargetTypeId);

            var report = new Report
            {
                TargetId = request.TargetId,
                TargetTypeId = request.TargetTypeId,
                ReasonTypeId = request.ReasonTypeId,
                Description = request.Description,
                CreatedBy = request.UserId,
                CreatedAt = now,
                ModerationTaskId = activeTask?.Id ?? null,
            };

            await _unitOfWork.Reports.AddAsync(report);
            await _unitOfWork.SaveChangesAsync();
            return new CreateReportResult(true, null);
        }
    }

}
