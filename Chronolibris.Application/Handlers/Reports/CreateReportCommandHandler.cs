using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Chronolibris.Application.Interfaces;
using Chronolibris.Application.Requests.Reports;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Options;
using FluentValidation;
using MediatR;

namespace Chronolibris.Application.Handlers.Reports
{
    public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, CreateReportResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ReportingOptions _options;
        private readonly IIdentityService _identityService;


        public CreateReportCommandHandler(
            IUnitOfWork unitOfWork,
            ReportingOptions options, IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _options = options;
            _identityService = identityService;
        }

        public async Task<CreateReportResult> Handle(
            CreateReportCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            bool userExists = await _identityService.IsUserActiveAsync(request.UserId);
            if (!userExists)
            {
                throw new ChronolibrisException("Нет доступа на совершение этой операции", ErrorType.Forbidden);
            }
            var now = DateTime.UtcNow;

            var cooldownThreshold = now - _options.ReportCooldown;

            var isOnCooldown = await _unitOfWork.Reports.GetLastUserReport(request.UserId,
                request.TargetTypeId, request.TargetId, request.ReasonTypeId, cancellationToken);
            
            if (isOnCooldown is not null && isOnCooldown.CreatedAt >= cooldownThreshold)
                throw new ChronolibrisException($"Вы уже отправляли подобную жалобу. Жалобы одного типа можно отправлять" +
                    $"не ранее, чем через {_options.ReportCooldown.TotalDays} дн.", ErrorType.TooManyRequests);

            var activeTask = await _unitOfWork.ModerationTasks.GetActiveByTarget(request.TargetId,
                request.TargetTypeId, cancellationToken);

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

            await _unitOfWork.Reports.AddAsync(report, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return new CreateReportResult(true, null);
        }
    }

}
