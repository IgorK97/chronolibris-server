using Chronolibris.Application.Handlers.Reports;
using Chronolibris.Application.Interfaces;
using Chronolibris.Application.Requests.Reports;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Options;
using FluentAssertions;
using Moq;

namespace ChronolibrisServer.Tests.Reports
{
    public class CreateReportCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IReportRepository> _reportRepoMock;
        private readonly Mock<IModerationTasksRepository> _tasksRepoMock;
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<ITransaction> _transactionMock;
        private readonly Mock<IReviewRepository> _reviewRepoMock;

        private const int UserId = 1;
        private const int TargetId = 10;
        private const int TargetTypeId = 2;
        private const int ReasonTypeId = 3;

        private Report? savedReport;

        private readonly ReportingOptions _defaultOptions;

        public CreateReportCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _reportRepoMock = new Mock<IReportRepository>();
            _tasksRepoMock = new Mock<IModerationTasksRepository>();
            _identityServiceMock = new Mock<IIdentityService>();
            _transactionMock = new Mock<ITransaction>();
            _reviewRepoMock = new Mock<IReviewRepository>();
            _defaultOptions = new ReportingOptions()
            {
                ReportCooldown = TimeSpan.FromDays(1)
            };
            SetupMocks();
        }

        private void SetupMocks()
        {
            _unitOfWorkMock.Setup(u => u.Reports).Returns(_reportRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.ModerationTasks).Returns(_tasksRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Reviews).Returns(_reviewRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);
            _identityServiceMock.Setup(i => i.IsUserActiveAsync(It.IsAny<long>())).ReturnsAsync(true);
            _transactionMock
                .Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _transactionMock
                .Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _transactionMock
                .Setup(t => t.DisposeAsync())
                .Returns(ValueTask.CompletedTask);
            _unitOfWorkMock
               .Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(_transactionMock.Object);

            _tasksRepoMock
                .Setup(r => r.GetActiveByTarget(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ModerationTask?)null);
            _reviewRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Review
            {
                BookId = 1,
                CreatedAt = DateTime.Now,
                Id = 10,
                //IsDeleted = false,
                DeletedAt = null,
                Score = 5,
                UserId = 1
            });


            _reportRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Report>(), It.IsAny<CancellationToken>()))
                .Callback<Report, CancellationToken>((r, _) => savedReport = r);
        }

        private void SetupTasksRepo(ModerationTask? task)
        {
            _tasksRepoMock.Setup(r => r.GetActiveByTarget(TargetId, TargetTypeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);
        }

        private void SetupReportsRepo(Report? report)
        {
            _reportRepoMock.Setup(r => r.GetLastUserReport(UserId, TargetTypeId, TargetId, ReasonTypeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(report);
        }
        private CreateReportCommandHandler CreateHandler()
        {
            return new CreateReportCommandHandler(_unitOfWorkMock.Object, _defaultOptions, _identityServiceMock.Object);
        }

        private CreateReportCommand CreateCommand(
            long userId = UserId,
            int targetId = TargetId,
            int targetTypeId = TargetTypeId,
            int reasonTypeId = ReasonTypeId,
            string description = "Спам")
        {
            return new CreateReportCommand(targetId, targetTypeId, reasonTypeId, description, userId);
        }

        [Fact]
        public async Task Handle_WithActiveTask_SetsTaskIdOnReport()
        {
            var activeTask = new ModerationTask { Id = 99 };

            //_reportRepoMock
            //    .Setup(r => r.GetLastUserReport(UserId, TargetTypeId, TargetId, ReasonTypeId, It.IsAny<CancellationToken>()))
            //    .ReturnsAsync((Report?)null);

            //_tasksRepoMock
            //    .Setup(r => r.GetActiveByTarget(TargetId, TargetTypeId, It.IsAny<CancellationToken>()))
            //    .ReturnsAsync(activeTask);

            SetupTasksRepo(activeTask);
            SetupReportsRepo(null);

            //Report? savedReport = null;
            //_reportRepoMock
            //    .Setup(r => r.AddAsync(It.IsAny<Report>(), It.IsAny<CancellationToken>()))
            //    .Callback<Report, CancellationToken>((r, _) => savedReport = r);

            var result = await CreateHandler().Handle(CreateCommand(), CancellationToken.None);

            savedReport.Should().NotBeNull();
            savedReport.ModerationTaskId.Should().Be(99);
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_WithoutActiveTask_LeavesTaskIdNull()
        {
            //_reportRepoMock
            //    .Setup(r => r.GetLastUserReport(UserId, TargetTypeId, TargetId, ReasonTypeId, It.IsAny<CancellationToken>()))
            //    .ReturnsAsync((Report?)null);

            //_tasksRepoMock
            //    .Setup(r => r.GetActiveByTarget(TargetId, TargetTypeId, It.IsAny<CancellationToken>()))
            //    .ReturnsAsync((ModerationTask?)null);

            SetupTasksRepo(null);
            SetupReportsRepo(null);

            //Report? savedReport = null;
            //_reportRepoMock
            //    .Setup(r => r.AddAsync(It.IsAny<Report>(), It.IsAny<CancellationToken>()))
            //    .Callback<Report, CancellationToken>((r, _) => savedReport = r);

            var result = await CreateHandler().Handle(CreateCommand(), CancellationToken.None);

            savedReport.Should().NotBeNull();
            savedReport.ModerationTaskId.Should().BeNull();
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ReportWithinCooldown_ThrowsTooManyRequests()
        {

            var recentReport = new Report
            {
                CreatedAt = DateTime.UtcNow - TimeSpan.FromMinutes(10)
            };
            SetupReportsRepo(recentReport);

            //_reportRepoMock
            //    .Setup(r => r.GetLastUserReport(UserId, TargetTypeId, TargetId, ReasonTypeId, It.IsAny<CancellationToken>()))
            //    .ReturnsAsync(recentReport);

            var act = () => CreateHandler().Handle(CreateCommand(), CancellationToken.None);

            await act.Should().ThrowAsync<ChronolibrisException>()
                .Where(e => e.ErrorType == ErrorType.TooManyRequests);
        }
    }
}