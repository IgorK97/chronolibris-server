using Chronolibris.Application.Handlers.Reports;
using Chronolibris.Application.Requests.Reports;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using FluentAssertions;
using Moq;

namespace ChronolibrisServer.Tests.Reports
{
    public class CreateModerationTaskHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IModerationTasksRepository> _taskRepoMock = new();
        private readonly Mock<IReportRepository> _reportRepoMock = new();
        private readonly Mock<ITransaction> _transactionMock = new();

        private readonly long targetId = 1;
        private readonly long moderatorId = 1;

        public CreateModerationTaskHandlerTests()
        {
            _unitOfWorkMock.Setup(u => u.ModerationTasks).Returns(_taskRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Reports).Returns(_reportRepoMock.Object);

            _unitOfWorkMock
            .Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_transactionMock.Object);

            _transactionMock
            .Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

            _transactionMock
                .Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _transactionMock
                .Setup(t => t.DisposeAsync())
                .Returns(ValueTask.CompletedTask);
        }

        private CreateModerationTaskCommand BuildCommand() => new(
            TargetId : targetId,
            TargetTypeId : 1,
            ModeratorId : moderatorId,
            ReportTypeId : 2
        );

        private CreateModerationTaskCommandHandler CreateHandler() =>
            new(_unitOfWorkMock.Object);

        [Fact]
        public async Task Handle_NoLastTask_CreatesTaskWithCheckNumberZero()
        {
            _taskRepoMock
                .Setup(r => r.GetLastTaskAsync(targetId, 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ModerationTask?)null);

            ModerationTask? captured = null;
            _taskRepoMock
                .Setup(r => r.TryCreateActiveTaskAsync(It.IsAny<ModerationTask>(), It.IsAny<CancellationToken>()))
                .Callback<ModerationTask, CancellationToken>((t, _) => captured = t)
                .ReturnsAsync(1);

            var result = await CreateHandler().Handle(BuildCommand(), CancellationToken.None);

            captured.Should().NotBeNull();
            captured.CheckNumber.Should().Be(1);
            captured.StatusId.Should().Be(2);
            captured.ModeratedBy.Should().Be(moderatorId);
            result.TaskStatusId.Should().Be(2);
        }

        [Fact]
        public async Task Handle_LastTaskExists_NotActive_IncrementsCheckNumber()
        {
            var lastTask = new ModerationTask { StatusId = 3, CheckNumber = 2 };

            _taskRepoMock
                .Setup(r => r.GetLastTaskAsync(targetId, 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(lastTask);

            ModerationTask? captured = null;
            _taskRepoMock
                .Setup(r => r.TryCreateActiveTaskAsync(It.IsAny<ModerationTask>(), It.IsAny<CancellationToken>()))
                .Callback<ModerationTask, CancellationToken>((t, _) => captured = t)
                .ReturnsAsync(2);

            var result = await CreateHandler().Handle(BuildCommand(), CancellationToken.None);

            captured.Should().NotBeNull();
            captured.CheckNumber.Should().Be(3);
            result.TaskStatusId.Should().Be(2);
        }

        [Fact]
        public async Task Handle_ActiveTaskExists_ThrowsConflict()
        {
            var activeTask = new ModerationTask { StatusId = 2 };

            _taskRepoMock
                .Setup(r => r.GetLastTaskAsync(targetId, 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(activeTask);

            var act = () => CreateHandler().Handle(BuildCommand(), CancellationToken.None);

            await act.Should().ThrowAsync<ChronolibrisException>()
                .Where(e => e.ErrorType == ErrorType.Conflict);

            _taskRepoMock.Verify(
                r => r.AddAsync(It.IsAny<ModerationTask>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
