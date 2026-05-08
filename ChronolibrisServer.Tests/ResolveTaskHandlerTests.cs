using Chronolibris.Application.Handlers.Reports;
using Chronolibris.Application.Requests.Reports;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using FluentAssertions;
using Moq;

namespace ChronolibrisServer.Tests.Reports
{
    public class ResolveTaskHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IModerationTasksRepository> _taskRepoMock = new();
        private readonly Mock<ICommentRepository> _commentRepoMock = new();
        private readonly Mock<IReviewRepository> _reviewRepoMock = new();
        private readonly Mock<IBookRepository> _bookRepoMock = new();
        private readonly Mock<ITransaction> _transactionMock = new();

        private readonly long moderatorId = 1;
        private readonly long taskId = 1;
        private readonly long targetId = 1;

        public ResolveTaskHandlerTests()
        {
            SetupMocks();
        }

        private void SetupMocks()
        {
            _unitOfWorkMock.Setup(u =>
                u.ModerationTasks).Returns(_taskRepoMock.Object);
            _unitOfWorkMock.Setup(u =>
                u.Comments).Returns(_commentRepoMock.Object);
            _unitOfWorkMock.Setup(u =>
                u.Reviews).Returns(_reviewRepoMock.Object);
            _unitOfWorkMock.Setup(u =>
                u.Books).Returns(_bookRepoMock.Object);

            _unitOfWorkMock.Setup(u =>
            u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_transactionMock.Object));

            _transactionMock.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _transactionMock.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _transactionMock.Setup(t => t.DisposeAsync())
                .Returns(ValueTask.CompletedTask);
        }

        private ResolveTaskCommand BuildCommand(bool resolution = true) =>
            new ResolveTaskCommand(taskId, resolution, moderatorId, "Test");

        private ModerationTask BuildTask(long targetTypeId = 3) =>
            new ModerationTask()
            {
                Id = taskId,
                ModeratedBy = moderatorId,
                StatusId = 2,
                TargetId = targetId,
                TargetTypeId = targetTypeId,
            };

        private ResolveTaskCommandHandler CreateHandler() =>
            new(_unitOfWorkMock.Object);

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task Handle_WrongStatus_ThrowsValidation(int statusId)//метод, усовие, результат, arrange, act, assert
        {
            var task = BuildTask();
            task.StatusId = statusId;
            _taskRepoMock
            .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);
            var act = () => CreateHandler().Handle(BuildCommand(), CancellationToken.None);

            await act.Should().ThrowAsync<ChronolibrisException>()
                .Where(e => e.ErrorType == ErrorType.Validation);
        }

        [Fact]
        public async Task Handle_ResolutionFalse_TaskRejected_ContentUntouched()
        {
            var task = BuildTask(targetTypeId: 3);
            _taskRepoMock
                .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);

            var result = await CreateHandler().Handle(
                BuildCommand(resolution: false), CancellationToken.None);

            result.Success.Should().BeTrue();
            task.StatusId.Should().Be(4); //Отклонены
        }

        [Fact]
        public async Task Handle_ResolutionTrue_CommentTarget_CommentDeleted()
        {
            var task = BuildTask(targetTypeId: 3);
            var comment = new Comment { Id = targetId,Text="Text",CreatedAt = DateTime.UtcNow, IsDeleted = false };

            _taskRepoMock
                .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);
            _commentRepoMock
                .Setup(r => r.GetByIdAsync(targetId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(comment);

            var result = await CreateHandler().Handle(BuildCommand(resolution: true), CancellationToken.None);

            result.Success.Should().BeTrue();
            task.StatusId.Should().Be(3); //Приняты
            comment.IsDeleted.Should().BeTrue();
            comment.DeletedAt.Should().NotBeNull();
        }
    }
}